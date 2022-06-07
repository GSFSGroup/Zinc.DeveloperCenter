import { AfterViewInit, Directive, ElementRef } from '@angular/core';

@Directive({
    selector: '[appAutoFocus]'
})
export class AutoFocusDirective implements AfterViewInit {
    public constructor(private element: ElementRef) {}

    public ngAfterViewInit(): void {
        setTimeout(() => {
            const htmlElement = this.element.nativeElement as HTMLElement;
            htmlElement.focus();
        });
    }
}
