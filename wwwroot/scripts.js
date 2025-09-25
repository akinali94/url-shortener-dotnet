async function shortenUrl() {
    const longUrl = document.getElementById('longUrl').value;
    if (!longUrl) {
        alert('Please enter a URL');
        return;
    }

    const response = await fetch('/api/urlshortener/shorten', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ longUrl })
    });

    const result = await response.json();
    if (response.ok) {
        document.getElementById('result').innerHTML = `Short URL: <a href="${result.shortUrl}" target="_blank">${result.shortUrl}</a>`;
    } else {
        document.getElementById('result').textContent = `Error: ${result.message}`;
    }
}
