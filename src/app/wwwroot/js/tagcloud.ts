const tagLinks = document.querySelectorAll('a.badge');

tagLinks.forEach((l: HTMLElement) => {
    // TODO: dynamic font size
    const count = parseInt(l.getAttribute('data-count'), 10);
});
