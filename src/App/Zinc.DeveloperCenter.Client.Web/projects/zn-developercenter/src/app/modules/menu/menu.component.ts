import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-menu',
    templateUrl: './menu.component.html',
    styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {
    public navSections = [
        {
            links: [
                {
                    htmlId: 'developer-center',
                    title: 'Developer Center',
                    routerLink: 'developer-center/developer-center'
                },
                {
                    htmlId: 'redline-adrs',
                    title: 'RedLine ADRs',
                    routerLink: 'developer-center/redline-adrs'
                }
            ]
        }
    ];
    public ngOnInit(): void {
        return;
    }
}
