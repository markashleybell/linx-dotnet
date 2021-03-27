export const greenIcon: { [index: number]: string } = {
    16: 'favicon-green-16x16.png',
    32: 'favicon-green-32x32.png',
    192: 'icon-green-192x192.png',
    512: 'icon-green-512x512.png',
};

export type MessageType =
    | 'pagedetailsrequest'
    | 'pagedetailsresponse'
    | 'linkexistsrequest'
    | 'linkexistsresponse';

export interface Message {
    readonly type: MessageType;
}

export class PageDetailsRequest implements Message {
    readonly type: MessageType;

    constructor() {
        this.type = 'pagedetailsrequest';
    }
}

export class PageDetailsResponse implements Message {
    readonly type: MessageType;

    constructor(public title: string, public url: string, public abstract: string) {
        this.type = 'pagedetailsresponse';
    }
}

export class LinkExistsRequest implements Message {
    readonly type: MessageType;

    constructor() {
        this.type = 'linkexistsrequest';
    }
}

export class LinkExistsResponse implements Message {
    readonly type: MessageType;

    constructor(public url: string, public linkExists: boolean) {
        this.type = 'linkexistsresponse';
    }
}

export interface ApiValidationError {
    key: string;
    value: string[];
}

export interface ApiErrorResponse {
    errors: ApiValidationError[];
}

export enum Setting {
    ApiUrl = 'api-url',
    ApiKey = 'api-key',
}

export interface SettingMetadata {
    name: string;
}

export const settings: Map<Setting, SettingMetadata> = new Map();

settings.set(Setting.ApiUrl, { name: 'API Endpoint URL' });
settings.set(Setting.ApiKey, { name: 'API Key' });

export function onMessageReceived(
    messageType: MessageType,
    callback: (message: Message, sender?: chrome.runtime.MessageSender) => void
) {
    chrome.runtime.onMessage.addListener((msg, sender) => {
        if (msg.type === messageType) {
            callback(msg, sender);
        }
    });
}

export function hideStatus(element: HTMLElement) {
    element.classList.add('status-hidden');
}

function showStatus(
    element: HTMLElement,
    message: string,
    statusClass: string,
    autoHideAfterMs?: number,
    onAfterHide?: () => void
) {
    element.classList.remove('status-hidden', 'bg-secondary', 'bg-success', 'bg-danger');

    element.classList.add(statusClass);
    element.innerText = message;

    if (autoHideAfterMs) {
        window.setTimeout(() => {
            hideStatus(element);
            if (onAfterHide) {
                onAfterHide();
            }
        }, autoHideAfterMs);
    }
}

export function showInfoStatus(
    element: HTMLElement,
    message: string,
    autoHideAfterMs?: number,
    onAfterHide?: () => void
) {
    showStatus(element, message, 'bg-secondary', autoHideAfterMs, onAfterHide);
}

export function showSuccessStatus(
    element: HTMLElement,
    message: string,
    autoHideAfterMs?: number,
    onAfterHide?: () => void
) {
    showStatus(element, message, 'bg-success', autoHideAfterMs, onAfterHide);
}

export function showErrorStatus(
    element: HTMLElement,
    message: string,
    autoHideAfterMs?: number,
    onAfterHide?: () => void
) {
    showStatus(element, message, 'bg-danger', autoHideAfterMs, onAfterHide);
}

export async function post(url: string, options: RequestInit) {
    const response = await fetch(url, options);
    const json = await response.json();
    return response.ok ? json : Promise.reject(json);
}

export function validateSettings(settings: { [key: string]: any }): [boolean, string, string] {
    const apiUrl = settings[Setting.ApiUrl]?.trim();
    const apiKey = settings[Setting.ApiKey]?.trim();

    return apiUrl && apiKey ? [true, apiUrl, apiKey] : [false, null, null];
}
