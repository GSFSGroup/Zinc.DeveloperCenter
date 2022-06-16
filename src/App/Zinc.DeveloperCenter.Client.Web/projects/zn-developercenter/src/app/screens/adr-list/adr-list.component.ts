import { RepositionScrollStrategy } from '@angular/cdk/overlay';
import { Component, OnInit } from '@angular/core';

import { ADRSummary, Repo } from '~/models/repo.interface';

@Component({
    selector: 'app-adr-list',
    templateUrl: './adr-list.component.html',
    styleUrls: ['./adr-list.component.scss']
})
export class AdrListComponent implements OnInit {

    // list of repos
    public repos!: Repo[];

    public constructor() { }

    public ngOnInit(): void {
        // populate repo accordions
        this.repos = this.constructRepoList(1);
        for (var i = 2; i < 5; i++) {
            this.repos.concat(this.constructRepoList(i));
        }
        // get universal ADRs
        this.getADRsForTemplateRepo();
        // get repo specific ADRs for individual repos
    }

    // constructs an array of repos
    // from the GSFS GitHub group
    // these repos are used to populate
    // the accordions on the ADR list page

    // NOTE: the GSFS group only contains 132 repos
    // as of 6/16/2022
    // the GitHub API can only retrieve 100 repos per query
    // this function is called on initialization 4 times
    // which means 400 repos are accounted for
    // if GSFS ever has more than 400, not all repos will be included
    // this will likely never happen.
    public constructRepoList(x: number): Repo[] {
        var repoList: Repo[] = [];

        // construct GitHub API query
        const xhr = new XMLHttpRequest();
        const url = `https://api.github.com/orgs/GSFSGroup/repos?per_page=100&page=`;

        xhr.open('GET', url.concat(x.toString()), true);
        xhr.setRequestHeader("Authorization",`token ghp`);
        
        xhr.onload = function() {
            const data = JSON.parse(this.response);
            console.log(data);
            // populate repos with each repo from query
            for (let i in data) {
                var idxPeriod = data[i].name.indexOf(".");
                var tempNeatName = data[i].name;
                var element_ = "";
                if (data[i].name.indexOf(".") != -1 && data[i].name[0].toUpperCase() == data[i].name[0]) {
                    tempNeatName = data[i].name.substr(idxPeriod+1);
                    element_ = data[i].name.substr(0,idxPeriod)
                }
                
                const adr: ADRSummary = {
                    name: "x",
                };

                const adrList: ADRSummary[] = [adr];
                
                const repo: Repo = {
                    neatName: tempNeatName,
                    dotName: data[i].name,
                    element: element_,
                    numADRs: 0,
                    contentURL: data[i].contents_url,
                    adrList: adrList,
                };
                repoList.push(repo);
                // sort the list alphabetically before data is received
                repoList.sort((a,b) => a.neatName.localeCompare(b.neatName));
            }
        }

        xhr.send(null);
        
        return repoList;
    }

    // gets the ADRs from the Zinc.Template repo
    // these ADRs are universal to all GSFS repos
    // other repos will not display these ADRs
    public getADRsForTemplateRepo() {
        // construct GitHub API query
        const xhr = new XMLHttpRequest();
        const url = `https://api.github.com/repos/GSFSGroup/Zinc.Templates/contents/dotnet-5.0/docs/RedLine`;

        xhr.open('GET', url, true);
        xhr.setRequestHeader("Authorization",`token ghp`);
        
        xhr.onload = function() {
            const data = JSON.parse(this.response);
            console.log(data);
        }

        xhr.send(null);
    }
}
