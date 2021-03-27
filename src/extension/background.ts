import { greenIcon, onMessageReceived } from './common';

onMessageReceived('linkexists', async (_) => {
    const tabs = await chrome.tabs.query({
        currentWindow: true,
        active: true,
    });

    const currentTab = tabs[0];

    chrome.action.disable(currentTab.id);
    chrome.action.setIcon({ path: greenIcon, tabId: currentTab.id });
});
