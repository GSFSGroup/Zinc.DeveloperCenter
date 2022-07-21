INSERT INTO developercenter.application (
    application_name,
    application_display_name,
    application_element
) VALUES (
    'Zinc.SampleApplicationOne',
    'Sample Application One',
    'Zinc'
);

INSERT INTO developercenter.application (
    application_name,
    application_display_name,
    application_element
) VALUES (
    'Zinc.SampleApplicationTwo',
    'Sample Application Two',
    'Zinc'
);

INSERT INTO developercenter.architecture_decision_record (
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated
) VALUES (
    'Zinc.SampleApplicationOne',
    1,
    'adr-0001-record-architecture-decisions',
    'https://raw.githubusercontent.com/GSFSGroup/Zinc.Templates/master/dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md?token={0}',
    'https://github.com/GSFSGroup/Zinc.Templates/blob/master/dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md',
    '5/21/2018'
);

INSERT INTO developercenter.architecture_decision_record (
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated
) VALUES (
    'Zinc.SampleApplicationOne',
    2,
    'adr-0002-organize-solution-into-layers',
    'https://raw.githubusercontent.com/GSFSGroup/Zinc.Templates/master/dotnet-5.0/docs/RedLine/adr-0002-organize-solution-into-layers.md?token={0}',
    'https://github.com/GSFSGroup/Zinc.Templates/blob/master/dotnet-5.0/docs/RedLine/adr-0002-organize-solution-into-layers.md',
    '6/19/2019'
);

INSERT INTO developercenter.architecture_decision_record (
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated
) VALUES (
    'Zinc.SampleApplicationOne',
    3,
    'adr-0003-implement-outbox-on-web-requests',
    'https://raw.githubusercontent.com/GSFSGroup/Zinc.Templates/master/dotnet-5.0/docs/RedLine/adr-0003-implement-outbox-on-web-requests.md?token={0}',
    'https://github.com/GSFSGroup/Zinc.Templates/blob/master/dotnet-5.0/docs/RedLine/adr-0003-implement-outbox-on-web-requests.md',
    '7/4/2020'
);

INSERT INTO developercenter.architecture_decision_record (
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated
) VALUES (
    'Zinc.SampleApplicationTwo',
    1,
    'adr-0001-event-sourcing',
    'https://raw.githubusercontent.com/GSFSGroup/Molybdenum.Earnings/main/docs/App/adr-0001-event-sourcing.md?token={0}',
    'https://github.com/GSFSGroup/Molybdenum.Earnings/blob/main/docs/App/adr-0001-event-sourcing.md',
    '1/21/2022'
);

INSERT INTO developercenter.architecture_decision_record (
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated
) VALUES (
    'Zinc.SampleApplicationTwo',
    2,
    'adr-0002-select-an-infinite-scroll-library',
    'https://raw.githubusercontent.com/GSFSGroup/Molybdenum.Earnings/main/docs/App/adr-0002-select-an-infinite-scroll-library.md?token={0}',
    'https://github.com/GSFSGroup/Molybdenum.Earnings/blob/main/docs/App/adr-0002-select-an-infinite-scroll-library.md',
    '2/13/2022'
);
