export interface AdrSummary {
    applicationName: string;
    title: string;
    titleDisplay: string;
    // eslint-disable-next-line id-blacklist
    number: number;
    numberDisplay: string;
    filePath: string;
    lastUpdatedBy: string;
    lastUpdatedOn: Date;
    totalViews: number;
    content: string;
}
