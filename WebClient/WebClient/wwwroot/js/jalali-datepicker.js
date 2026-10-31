/*!
 * JalaliDatePicker – تقویم شمسی حرفه‌ای (بدون وابستگی)
 * Usage:
 *   const dp = new JalaliDatePicker(inputEl, { min: '1400/01/01', max: '1410/12/29', onChange(d){} });
 *   dp.getValue()  -> { jy, jm, jd } | null
 *   dp.setValue(1403, 7, 15)
 */
(function (global) {
  'use strict';

  const MONTHS = ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور',
                  'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'];
  const WEEKDAYS = ['ش', 'ی', 'د', 'س', 'چ', 'پ', 'ج']; // شنبه → جمعه
  const FA_DIGITS = '۰۱۲۳۴۵۶۷۸۹';

  const toFa = (n) => String(n).replace(/\d/g, (d) => FA_DIGITS[d]);
  const toEn = (s) => String(s).replace(/[۰-۹]/g, (d) => FA_DIGITS.indexOf(d))
                               .replace(/[٠-٩]/g, (d) => '٠١٢٣٤٥٦٧٨٩'.indexOf(d));

  // ---- Jalali <-> Gregorian (algorithm based on 33-year cycle, accurate 1178–1600 AP) ----
  function jalCal(jy) {
    const breaks = [-61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210, 1635, 2060,
                    2097, 2192, 2262, 2324, 2394, 2456, 3178];
    let bl = breaks.length, gy = jy + 621, leapJ = -14, jp = breaks[0], jm, jump, leap, n, i;
    if (jy < jp || jy >= breaks[bl - 1]) throw new RangeError('Invalid Jalali year ' + jy);
    for (i = 1; i < bl; i++) {
      jm = breaks[i];
      jump = jm - jp;
      if (jy < jm) break;
      leapJ = leapJ + Math.floor(jump / 33) * 8 + Math.floor((jump % 33) / 4);
      jp = jm;
    }
    n = jy - jp;
    leapJ = leapJ + Math.floor(n / 33) * 8 + Math.floor(((n % 33) + 3) / 4);
    if ((jump % 33) === 4 && (jump - n) === 4) leapJ += 1;
    const leapG = Math.floor(gy / 4) - Math.floor((Math.floor(gy / 100) + 1) * 3 / 4) - 150;
    const march = 20 + leapJ - leapG;
    if (jump - n < 6) n = n - jump + Math.floor((jump + 4) / 33) * 33;
    leap = ((n + 1) % 33 - 1);
    if (leap === -1) leap = 4;
    return { leap, gy, march };
  }

  // Gregorian date -> Julian Day Number (standard integer formula)
  function g2d(gy, gm, gd) {
    const a = Math.floor((14 - gm) / 12);
    const y = gy + 4800 - a;
    const m = gm + 12 * a - 3;
    return gd + Math.floor((153 * m + 2) / 5) + 365 * y + Math.floor(y / 4)
         - Math.floor(y / 100) + Math.floor(y / 400) - 32045;
  }

  function d2g(jdn) {
    // inverse of g2d: find the Gregorian year, then month/day
    let gy = Math.floor((jdn - 1721060) / 365.2425) + 1;
    while (g2d(gy + 1, 1, 1) <= jdn) gy++;
    while (g2d(gy, 1, 1) > jdn) gy--;
    let gm = 1;
    while (gm < 12 && g2d(gy, gm + 1, 1) <= jdn) gm++;
    const gd = jdn - g2d(gy, gm, 1) + 1;
    return { gy, gm, gd };
  }

  function jalToJdn(jy, jm, jd) {
    const { march } = jalCal(jy);
    let jdn = g2d(jy + 621, 3, march) + (jm - 1) * 31 - Math.floor(jm / 7) * (jm - 7) + (jd - 1);
    return jdn;
  }

  function jdnToJal(jdn) {
    const g = d2g(jdn);
    const jy = g.gy - 621;
    let jyF = jy;
    if (jdn >= jalToJdn(jy + 1, 1, 1)) jyF = jy + 1;
    else if (jdn < jalToJdn(jy, 1, 1)) jyF = jy - 1;
    const day = jdn - jalToJdn(jyF, 1, 1);
    let jm, jd;
    if (day < 186) { jm = Math.floor(day / 31) + 1; jd = (day % 31) + 1; }
    else { jm = Math.floor((day - 186) / 30) + 7; jd = ((day - 186) % 30) + 1; }
    return { jy: jyF, jm, jd };
  }

  function jalaliDaysInMonth(jy, jm) {
    if (jm <= 6) return 31;
    if (jm <= 11) return 30;
    // esfand length = distance between this Nowruz and the next one
    return jalToJdn(jy + 1, 1, 1) - jalToJdn(jy, 12, 1);
  }

  function todayJal() {
    const n = new Date();
    const jdn = Math.floor(Date.UTC(n.getFullYear(), n.getMonth(), n.getDate()) / 86400000) + 2440588;
    return jdnToJal(jdn);
  }

  // weekday: 0 = Saturday … 6 = Friday
  function weekdayOf(jy, jm, jd) {
    const jdn = jalToJdn(jy, jm, jd);
    return (jdn + 2) % 7;
  }

  function cmp(a, b) { return a.jy - b.jy || a.jm - b.jm || a.jd - b.jd; }

  function parseDate(str) {
    const m = toEn(str || '').trim().match(/^(\d{3,4})[\/\-](\d{1,2})[\/\-](\d{1,2})$/);
    if (!m) return null;
    return { jy: +m[1], jm: +m[2], jd: +m[3] };
  }

  function fmt(d) {
    return d.jy + '/' + String(d.jm).padStart(2, '0') + '/' + String(d.jd).padStart(2, '0');
  }

  // ---- Component ----
  let uid = 0;
  const openInstances = new Set();
  document.addEventListener('mousedown', (e) => {
    openInstances.forEach((dp) => {
      if (!dp.root.contains(e.target) && e.target !== dp.input) dp.close();
    });
  });

  class JalaliDatePicker {
    constructor(input, opts = {}) {
      this.input = typeof input === 'string' ? document.querySelector(input) : input;
      this.opts = Object.assign({ format: 'yyyy/mm/dd', persianDigits: true, onChange: null }, opts);
      this.min = opts.min ? parseDate(opts.min) : null;
      this.max = opts.max ? parseDate(opts.max) : null;
      this.value = parseDate(this.input.value);
      const t = todayJal();
      this.view = this.value ? { jy: this.value.jy, jm: this.value.jm } : { jy: t.jy, jm: t.jm };
      this.today = t;
      this.id = 'jdp' + (++uid);
      this.isOpen = false;
      this._build();
      this._bind();
      this._render();
    }

    _build() {
      this.input.setAttribute('readonly', 'readonly');
      this.input.setAttribute('autocomplete', 'off');
      this.input.setAttribute('aria-haspopup', 'dialog');
      this.root = document.createElement('div');
      this.root.className = 'jdp';
      this.root.setAttribute('dir', 'rtl');
      this.root.setAttribute('role', 'dialog');
      this.root.setAttribute('aria-label', 'انتخاب تاریخ');
      this.root.innerHTML = `
        <div class="jdp-header">
          <button type="button" class="jdp-btn" data-act="prevY" aria-label="سال قبل">«</button>
          <button type="button" class="jdp-btn" data-act="prevM" aria-label="ماه قبل">‹</button>
          <div class="jdp-title">
            <select class="jdp-sel jdp-sel-m" aria-label="ماه"></select>
            <select class="jdp-sel jdp-sel-y" aria-label="سال"></select>
          </div>
          <button type="button" class="jdp-btn" data-act="nextM" aria-label="ماه بعد">›</button>
          <button type="button" class="jdp-btn" data-act="nextY" aria-label="سال بعد">»</button>
        </div>
        <div class="jdp-week">${WEEKDAYS.map((w) => `<span>${w}</span>`).join('')}</div>
        <div class="jdp-grid" role="grid"></div>
        <div class="jdp-footer">
          <button type="button" class="jdp-link" data-act="today">امروز</button>
          <button type="button" class="jdp-link" data-act="clear">پاک کردن</button>
          <button type="button" class="jdp-link" data-act="close">بستن</button>
        </div>`;
      this.selM = this.root.querySelector('.jdp-sel-m');
      this.selY = this.root.querySelector('.jdp-sel-y');
      this.grid = this.root.querySelector('.jdp-grid');
      MONTHS.forEach((name, i) => this.selM.add(new Option(name, i + 1)));
      for (let y = this.today.jy - 100; y <= this.today.jy + 20; y++) {
        this.selY.add(new Option(this.opts.persianDigits ? toFa(y) : y, y));
      }
      this.input.insertAdjacentElement('afterend', this.root);
      this.root.style.display = 'none';
      this.root.classList.add('jdp');
    }

    _bind() {
      this.input.addEventListener('click', () => this.open());
      this.input.addEventListener('focus', () => this.open());
      this.input.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' || e.key === ' ' || e.key === 'ArrowDown') { e.preventDefault(); this.open(); }
        if (e.key === 'Escape') this.close();
      });
      this.root.addEventListener('click', (e) => {
        const btn = e.target.closest('[data-act]');
        const dayEl = e.target.closest('[data-day]');
        if (dayEl && !dayEl.disabled) return this._select(+dayEl.dataset.jy, +dayEl.dataset.jm, +dayEl.dataset.day);
        if (!btn) return;
        switch (btn.dataset.act) {
          case 'prevY': this._shift(-12); break;
          case 'nextY': this._shift(12); break;
          case 'prevM': this._shift(-1); break;
          case 'nextM': this._shift(1); break;
          case 'today': this._select(this.today.jy, this.today.jm, this.today.jd); break;
          case 'clear': this.setValue(null); break;
          case 'close': this.close(); break;
        }
      });
      this.selM.addEventListener('change', () => { this.view.jm = +this.selM.value; this._render(); });
      this.selY.addEventListener('change', () => { this.view.jy = +this.selY.value; this._render(); });
      this.root.addEventListener('keydown', (e) => this._onKey(e));
    }

    _onKey(e) {
      const map = { ArrowLeft: 1, ArrowRight: -1, ArrowUp: -7, ArrowDown: 7 };
      if (e.key === 'Escape') { this.close(); this.input.focus(); return; }
      if (e.key === 'PageUp' || e.key === 'PageDown') {
        e.preventDefault();
        return this._shift(e.key === 'PageUp' ? -1 : 1);
      }
      if (!(e.key in map) && e.key !== 'Enter') return;
      e.preventDefault();
      const base = this.focusDate || this.value || this.today;
      if (e.key === 'Enter') return this._select(base.jy, base.jm, base.jd);
      const step = { ArrowLeft: 1, ArrowRight: -1, ArrowUp: -7, ArrowDown: 7 }[e.key];
      const n = jdnToJal(jalToJdn(base.jy, base.jm, base.jd) + step);
      this.focusDate = n;
      this.view = { jy: n.jy, jm: n.jm };
      this._render();
      this.root.querySelector('[data-focus]')?.focus();
    }

    _shift(months) {
      let jm = this.view.jm + months, jy = this.view.jy;
      while (jm > 12) { jm -= 12; jy++; }
      while (jm < 1) { jm += 12; jy--; }
      this.view = { jy, jm };
      this._render();
    }

    _isDisabled(d) {
      return (this.min && cmp(d, this.min) < 0) || (this.max && cmp(d, this.max) > 0);
    }

    _render() {
      const { jy, jm } = this.view;
      this.selM.value = jm;
      if (![...this.selY.options].some((o) => +o.value === jy)) this.selY.add(new Option(jy, jy));
      this.selY.value = jy;

      const n = jalaliDaysInMonth(jy, jm);
      const startOffset = weekdayOf(jy, jm, 1); // 0 = شنبه
      const focus = this.focusDate && this.focusDate.jy === jy && this.focusDate.jm === jm
        ? this.focusDate.jd : (this.value && this.value.jy === jy && this.value.jm === jm ? this.value.jd : 1);

      let html = '';
      for (let i = 0; i < startOffset; i++) html += '<span class="jdp-empty"></span>';
      for (let d = 1; d <= n; d++) {
        const cur = { jy, jm, jd: d };
        const classes = ['jdp-day'];
        if (this.value && cmp(cur, this.value) === 0) classes.push('is-selected');
        if (cmp(cur, this.today) === 0) classes.push('is-today');
        const dis = this._isDisabled(cur);
        if (dis) classes.push('is-disabled');
        if (weekdayOf(jy, jm, d) === 6) classes.push('is-friday'); // جمعه
        html += `<button type="button" class="${classes.join(' ')}" data-day="${d}" data-jy="${jy}" data-jm="${jm}"
                  ${dis ? 'disabled' : ''} ${d === focus ? 'data-focus tabindex="0"' : 'tabindex="-1"'}
                  aria-label="${toFa(d)} ${MONTHS[jm - 1]} ${toFa(jy)}"
                  aria-selected="${this.value && cmp(cur, this.value) === 0}">${this.opts.persianDigits ? toFa(d) : d}</button>`;
      }
      this.grid.innerHTML = html;
    }

    _select(jy, jm, jd) {
      const d = { jy, jm, jd };
      if (this._isDisabled(d)) return;
      this.setValue(d);
      this.close();
      this.input.focus();
    }

    setValue(d, silent) {
      this.value = d ? { jy: d.jy, jm: d.jm, jd: d.jd } : null;
      this.focusDate = null;
      if (this.value) this.view = { jy: this.value.jy, jm: this.value.jm };
      this.input.value = this.value
        ? (this.opts.persianDigits ? toFa(fmt(this.value)) : fmt(this.value))
        : '';
      this._render();
      if (!silent && typeof this.opts.onChange === 'function') this.opts.onChange(this.getValue());
    }

    getValue() { return this.value ? { ...this.value } : null; }
    getGregorian() {
      if (!this.value) return null;
      const jdn = jalToJdn(this.value.jy, this.value.jm, this.value.jd);
      return d2g(jdn);
    }

    open() {
      if (this.isOpen) return;
      openInstances.forEach((dp) => { if (dp !== this) dp.close(); });
      this.isOpen = true;
      this.root.style.display = 'block';
      this.focusDate = null;
      this.view = this.value ? { jy: this.value.jy, jm: this.value.jm } : { jy: this.today.jy, jm: this.today.jm };
      this._render();
      openInstances.add(this);
    }

    close() {
      if (!this.isOpen) return;
      this.isOpen = false;
      this.root.style.display = 'none';
      openInstances.delete(this);
    }

    destroy() {
      this.close();
      this.root.remove();
      this.input.removeAttribute('readonly');
    }
  }

  global.JalaliDatePicker = JalaliDatePicker;
  global.JalaliDate = { toJdn: jalToJdn, fromJdn: jdnToJal, g2d, d2g, daysInMonth: jalaliDaysInMonth, toFa, toEn };
})(typeof window !== 'undefined' ? window : globalThis);
