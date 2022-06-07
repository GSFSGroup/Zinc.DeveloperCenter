import { DOCUMENT } from '@angular/common';
import { Inject, Injectable, OnDestroy } from '@angular/core';

/** Container inside which all overlays will render. */
@Injectable({ providedIn: 'root' })
export class CustomOverlayContainer implements OnDestroy {
    protected _document: Document;

    protected get containerElement(): HTMLElement | null {
        return document.querySelector('.cdk-overlay-container');
    }

    public constructor(@Inject(DOCUMENT) document: any) {
        this._document = document;
    }

    // required as testing calls the method.
    // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method,
    public ngOnDestroy() {}

    /**
     * This method returns the overlay container element. It will lazily
     * create the element the first time  it is called to facilitate using
     * the container in non-browser environments.
     *
     * @returns the container element
     */
    public getContainerElement(): HTMLElement | null {
        if (!this.containerElement) {
            this._createContainer();
        }

        return this.containerElement;
    }

    /**
     * Create the overlay container element, which is simply a div
     * with the 'cdk-overlay-container' class on the document body.
     */
    protected _createContainer(): void {
        const container = this._document.createElement('div');
        container.classList.add('cdk-overlay-container');
        this._document.body.appendChild(container);
    }
}
