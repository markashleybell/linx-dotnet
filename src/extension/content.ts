import {
    LinkExists,
    onMessageReceived,
    PageDetails,
    post,
    settings,
    validateSettings,
} from './common';

onMessageReceived('pagedetailsrequest', (_) => {
    const message = new PageDetails(
        document.title,
        window.location.href,
        window.getSelection().toString()
    );
    chrome.runtime.sendMessage(message);
});

window.addEventListener('load', (_) => {
    chrome.storage.sync.get([...settings.keys()], async (stored) => {
        const [settingsAreValid, apiUrl, apiKey] = validateSettings(stored);

        if (!settingsAreValid) {
            return;
        }

        const currentUrl = window.location.href;

        const checkUrl = apiUrl + '/check';

        var formData = new FormData();

        formData.append('Url', currentUrl);

        const response = await post(checkUrl, {
            method: 'POST',
            body: formData,
            headers: {
                ApiKey: apiKey,
            },
        });

        if (response.exists) {
            const message = new LinkExists(currentUrl);
            chrome.runtime.sendMessage(message);
        }
    });
});
