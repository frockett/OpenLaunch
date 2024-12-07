window.updateTheme = (href) => {
    const link = document.getElementById('themeStylesheet');
    if (link) {
        link.href = href;
    }
};