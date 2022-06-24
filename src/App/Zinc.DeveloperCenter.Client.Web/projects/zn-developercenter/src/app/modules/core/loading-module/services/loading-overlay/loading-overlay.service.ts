import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subject, timer } from 'rxjs';
import { distinctUntilChanged, takeUntil } from 'rxjs/operators';

import { LoadingOverlayComponent } from '~/core/loading-module/components/loading-overlay/loading-overlay.component';

@Injectable({
    providedIn: 'root'
})
export class LoadingOverlayService implements OnDestroy {
    private loading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    private overlayRef: OverlayRef | null = null;
    private destroy$: Subject<boolean> = new Subject<boolean>();
    private message = 'Loading';
    private showLoading = false;
    private waitDuration = 1000;
    private maxDuration = 10000;

    public constructor(private overlay: Overlay) {
        this.loading$.pipe(distinctUntilChanged(), takeUntil(this.destroy$)).subscribe(loading => {
            if (loading) {
                this.showLoading = true;
                timer(this.waitDuration)
                    .pipe(takeUntil(this.destroy$))
                    .subscribe(() => {
                        if (this.showLoading) {
                            this.showOverlay(this.message);
                            setTimeout(() => this.loading$.next(false), this.maxDuration);
                        }
                    });
            } else {
                this.showLoading = false;
                this.removeOverlay();
            }
        });
    }

    public ngOnDestroy(): void {
        this.hide();
        this.destroy$.next();
        this.destroy$.complete();
    }

    public show(message: string, waitDuration?: number, maxDuration?: number) {
        this.message = message;
        this.waitDuration = waitDuration ?? this.waitDuration;
        this.maxDuration = maxDuration ?? this.maxDuration;
        this.loading$.next(true);
    }

    public hide() {
        this.loading$.next(false);
    }

    private showOverlay(message: string) {
        if (!this.overlayRef) {
            this.overlayRef = this.overlay.create();
        }

        const loadingPortal = new ComponentPortal(LoadingOverlayComponent);
        const componentRef = this.overlayRef.attach(loadingPortal);
        componentRef.instance.message = message;
        componentRef.changeDetectorRef.detectChanges();
    }

    private removeOverlay() {
        this.message = 'Loading';

        if (!!this.overlayRef) {
            this.overlayRef.detach();
        }
    }
}
