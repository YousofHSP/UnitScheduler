const pad = n => String(n).padStart(2, '0');
const parse = s => {
    const p = s?.split('/').map(Number);
    return p?.length === 3 ? { jy: p[0], jm: p[1], jd: p[2] } : null;
};

window.jdpInterop = {
    create(input, opts, dotnet) {
        const dp = new JalaliDatePicker(input, {
            min: opts.min || undefined,
            max: opts.max || undefined,
            onChange: d => dotnet.invokeMethodAsync('OnPicked',
                d ? `${d.jy}/${pad(d.jm)}/${pad(d.jd)}` : null)
        });
        input._jdp = dp;
        if (opts.value) dp.setValue(parse(opts.value), true);
    },
    setValue(input, value) { input._jdp?.setValue(parse(value), true); },
    destroy(input) { input._jdp?.destroy(); }
};