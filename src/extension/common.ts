export interface LinxChromeExtension {
    getPageDetails(callback: (message: any) => void): void;
}

export interface Window {
    LinxChromeExtension: LinxChromeExtension;
}

export type Setting  = 'api-url' | 'api-key';

export interface SettingMetadata {
    name: string;
}

export const settings: Map<Setting, SettingMetadata> = new Map();

settings.set('api-url', { name: 'API Endpoint URL' });
settings.set('api-key', { name: 'API Key' });

export function showStatus(container: HTMLElement, status: string, delay?: number) {
    container.innerHTML = status;
    window.setTimeout(() => { 
        container.innerHTML = ''; 
    }, delay || 1000);
}
