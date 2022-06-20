export interface Repo {
    neatName: string;
    dotName: string;
    element: string,
    numADRs: number;
    contentURL: string;
    adrList: ADRSummary[];
    index: number;
}

export interface ADRSummary {
    name: string;
}
