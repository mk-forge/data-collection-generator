function setFavicon(url) {
    let old = document.getElementById('dynamic-favicon');
    if (old) old.remove();

    let link = document.createElement('link');
    link.id = 'dynamic-favicon';
    link.rel = 'icon';
    link.type = 'image/png';
    link.href = url;
    document.head.appendChild(link);
};