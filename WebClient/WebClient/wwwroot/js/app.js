window.initJalaliDatePicker = () => {
    jalaliDatepicker.startWatch({});
};
window.detailsOpenInterop = {
    openParentDetails: function () {
        // همه NavLink های فعال را پیدا کن
        const activeLinks = document.querySelectorAll('.menu-active');
        activeLinks.forEach(link => {
            let parentDetails = link.closest('details');
            if (parentDetails) {
                parentDetails.setAttribute('open', '');
            }
        });
    }
};
window.addEventListener('DOMContentLoaded', () => {
    jalaliDatepicker.startWatch({
        selector: '[data-jdp], [data-jdp-datetime]',
        date: true,
        time: false, // به صورت پیش‌فرض false
        showSecond: false,
        persianDigits: true,
        autoShow: true,
        autoHide: true,
        separatorChars: {
            date: '/',
            time: ':',
            between: ' '
        },
        onShow: (el) => {
            if (el.hasAttribute('data-jdp-datetime')) {
                el.jalaliDatepicker.time = true; // فعال کردن time برای inputهای datetime
            }
        }
    });
    document.querySelectorAll('input[data-jdp]').forEach(el => {
        el.setAttribute('autocomplete', 'off');
        el.setAttribute('autocorrect', 'off');
        el.setAttribute('autocapitalize', 'off');
        el.setAttribute('spellcheck', 'false');
    });
})

window.openDialog = (id) => {
    const modal = document.getElementById(id);
    if (modal) {
        modal.showModal();
    }
}

window.close = (id) => {
    const modal = document.getElementById(id);
    if (modal) {
        modal.close();
    }
}
window.openModal = (id) => {
    const modal = document.getElementById(id);
    if (modal) {
        modal.show();
        const input = modal.querySelector('[data-jdp]');
        if (input) {
            jalaliDatepicker.startWatch(input, {
                container: document.querySelector('dialog'),
                autoShow: true,
                autoHide: true,
                persianDigits: true
            });
        }
    }
}
window.closeModal = (id) => {
    const modal = document.getElementById(id);
    if (modal) {
        modal.close();
    }
}

// === toggle sidebar ===
window.toggleSidebar = () => {
    const wrapper = document.getElementById("layoutWrapper");
    if (wrapper) {
        wrapper.classList.toggle("sidebar-collapsed");
    }
}

window.downloadFileFromBytes = (fileName, byteArray) => {
    const blob = new Blob([new Uint8Array(byteArray)], {type: "application/octet-stream"});
    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();

    URL.revokeObjectURL(url);
    link.remove();
};

async function downloadFileFromStream(fileName, contentStreamReference) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName;
    anchor.click();
    URL.revokeObjectURL(url);
}

window.triggerClick = (elementId) => {
    var element = document.getElementById(elementId);
    if (element) element.click();
};
window.triggerClickByElement = (element) => {
    if (element) {
        element.click();
    }
};

window.openDropdown = (buttonId, dropdownId) => {
    const button = document.getElementById(buttonId);
    const dropdown = document.getElementById(dropdownId);

    const rect = button.getBoundingClientRect();

    dropdown.style.position = "fixed";
    dropdown.style.top = rect.top + "px";
    dropdown.style.left = rect.right + "px";

    dropdown.style.zIndex = 9999999;

    dropdown.classList.remove("hidden");
};

// تابع جدید برای بستن منو
window.closeDropdown = (dropdownId) => {
    const dropdown = document.getElementById(dropdownId);
    if (dropdown) {
        dropdown.classList.add("hidden");
    }
};