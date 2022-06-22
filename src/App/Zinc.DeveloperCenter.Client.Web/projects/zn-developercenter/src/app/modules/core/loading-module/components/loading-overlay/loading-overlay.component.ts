import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

@Component({
    selector: 'app-loading-overlay',
    templateUrl: './loading-overlay.component.html',
    styleUrls: ['./loading-overlay.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoadingOverlayComponent {
    @Input()
    public message: string | undefined;

    public constructor() {}
}
