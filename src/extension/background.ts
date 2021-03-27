import { greenIcon, LinkExistsResponse, LinkExistsRequest, onMessageReceived } from './common';

onMessageReceived('linkexistsresponse', (msg: LinkExistsResponse, sender) => {
    if (msg.linkExists) {
        chrome.action.disable(sender.tab.id);
        chrome.action.setIcon({ path: greenIcon, tabId: sender.tab.id });
    }
});

chrome.tabs.onActivated.addListener((tab) => {
    chrome.tabs.sendMessage(tab.tabId, new LinkExistsRequest());
});
