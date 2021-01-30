import { Setting, settings, showStatus } from './common';

const ui = document.getElementById('settings-ui');
const status = document.getElementById('status');
const saveButton = document.getElementById('save');

const inputs: Map<Setting, HTMLInputElement> = new Map();

function uiElements(setting: Setting, label: string, value?: string): [HTMLElement, HTMLInputElement] {
    const containerTemplate = document.createElement('template');
    containerTemplate.innerHTML = `<p><label for="${setting}">${label}</label><br></p>`;

    const inputTemplate = document.createElement('template');
    inputTemplate.innerHTML = `<input type="text" id="${setting}" name="${setting}" value="${value || ''}"/>`;

    const input = inputTemplate.content.firstChild.cloneNode();

    containerTemplate.content.firstChild.appendChild(input);

    return [containerTemplate.content.firstChild as HTMLElement, input as HTMLInputElement];
}

window.addEventListener("load", _ => {
    chrome.storage.sync.get([...settings.keys()], stored => {
        for (const [setting, metaData] of settings) {
            const [container, input] = uiElements(setting, metaData.name, stored[setting]);
            inputs.set(setting, input);
            ui.appendChild(container);
        }
    });
});

saveButton.addEventListener('click', _ => {
    const storageValues = [...inputs].reduce((o: any, [k, v]) => { o[k] = v.value; return o; }, {});
    chrome.storage.sync.set(storageValues, () => {
        showStatus(status, 'Settings saved');
    });
});
