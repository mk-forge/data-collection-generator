let lastUrl = location.href;
applyTheme();

function applyTheme() {
    let theme = localStorage.getItem("theme") || "light";
    document.documentElement.classList.toggle("dark-mode", theme == "dark");
    document.documentElement.classList.toggle("light-mode", theme == "light");
}

function toggleTheme() {
    let theme = localStorage.getItem("theme") == "dark" ? "light" : "dark";
    localStorage.setItem("theme", theme);
    applyTheme();

    if (window.histogramData && window.histogramPage && window.useIntervals)
        drawHistogram(window.histogramData, window.histogramPage, window.useIntervals);
}

function changeTheme() {
    if (location.href != lastUrl) {
        lastUrl = location.href;
        applyTheme();
    }
}

new MutationObserver(changeTheme).observe(document, { childList: true, subtree: true });