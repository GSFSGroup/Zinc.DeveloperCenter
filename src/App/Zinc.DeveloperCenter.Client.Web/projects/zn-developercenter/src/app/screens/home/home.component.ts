import { Component } from '@angular/core';

import { HomeService } from './home.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent {
    public constructor(private homeService: HomeService) {
    }

    public testClickFunction() {
        alert('clicked Read More button.');
    }
}
