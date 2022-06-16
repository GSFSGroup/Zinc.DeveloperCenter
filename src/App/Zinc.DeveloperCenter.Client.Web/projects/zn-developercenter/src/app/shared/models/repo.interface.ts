export interface Repo {
    neatName: string;
    dotName: string;
    element: string,
    numADRs: number;
    contentURL: string;
    adrList: ADRSummary[];
}

export interface ADRSummary {
    name: string;
}
