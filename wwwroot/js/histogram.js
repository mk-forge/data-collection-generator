function resetHistogramWidth() {
    window.maxTextWidth = null;
}

function drawHistogram(data, page, useIntervals) {
    window.histogramData = data;
    window.histogramPage = page

    let canvas = document.getElementById("histogramCanvas");
    let ctx = canvas.getContext("2d");
    ctx.font = "17px Inter";

    if (!window.maxTextWidth && window.histogramData) {
        let maxTextWidth = 0;
        for (let item of window.histogramData) {
            let textWidth = ctx.measureText(item.uniqueCount.toString()).width;
            if (textWidth > maxTextWidth)
                maxTextWidth = textWidth;
        }
        window.maxTextWidth = maxTextWidth;
    }

    let maxTextWidth = window.maxTextWidth;

    let barW = Math.max(50, maxTextWidth + 10);
    let gap = 50, leftPad = 40, rightPad = 40;
    canvas.width = leftPad + data.length * (barW + gap) - gap + rightPad;
    canvas.height = 400;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.textBaseline = "alphabetic";

    let darkMode = document.documentElement.classList.contains("dark-mode");
    let textColor = darkMode ? "white" : "#212529";
    let maxUnique = Math.max(...data.map(x => x.uniqueCount));
    let scale = (canvas.height - 60) / maxUnique;

    let colors = {
        bar: "#3498db",
        text: textColor,
        count: "white",
    };

    for (let i = 0; i < data.length; i++) {
        let item = data[i];
        let barH;
        let fullH = canvas.height - 60;
        let scale = fullH / maxUnique;

        if (item.uniqueCount == 0)
            barH = scale * 0.5;
        else
            barH = item.uniqueCount * scale;

        barH = Math.max(barH, 15);

        let barX = leftPad + i * (barW + gap);
        let barY = canvas.height - barH - 30;

        ctx.fillStyle = colors.bar;
        ctx.fillRect(barX, barY, barW, barH);

        ctx.font = "17px Inter";
        ctx.textAlign = "center";

        ctx.fillStyle = colors.count;
        ctx.textBaseline = "middle";
        ctx.fillText(item.uniqueCount, barX + barW / 2, barY + barH / 2);

        ctx.fillStyle = colors.text;
        let label = useIntervals ? item.interval : item.dimension;
        ctx.fillText(label, barX + barW / 2, canvas.height - 10);
    }
}