window.scheduleDrag = {
    attach: function (dotNetObj) {
        let dragging = true;
        let lastX = null, lastY = null;

        function handleMove(e) {
            if (!dragging) return;
            if (lastX === null || lastY === null) {
                lastX = e.clientX;
                lastY = e.clientY;
                return;
            }
            let deltaX = e.clientX - lastX;
            let deltaY = e.clientY - lastY;
            dotNetObj.invokeMethodAsync('OnDrag', deltaX, deltaY);
            lastX = e.clientX;
            lastY = e.clientY;
        }

        function handleUp(e) {
            dragging = false;
            window.removeEventListener('pointermove', handleMove);
            window.removeEventListener('pointerup', handleUp);
        }

        window.addEventListener('pointermove', handleMove);
        window.addEventListener('pointerup', handleUp);
    }
};