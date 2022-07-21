INSERT INTO developercenter.application (
    tenant_id,
    application_name,
    application_display_name,
    application_element
) VALUES (
    'GSFSGroup',
    'Zinc.SampleApplicationOne',
    'Sample Application One',
    'Zinc'
);

INSERT INTO developercenter.application (
    tenant_id,
    application_name,
    application_display_name,
    application_element
) VALUES (
    'GSFSGroup',
    'Zinc.SampleApplicationTwo',
    'Sample Application Two',
    'Zinc'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Zinc.SampleApplicationOne',
    1,
    'adr-0001-record-architecture-decisions',
    'https://api.github.com/repos/GSFSGroup/Zinc.Templates/contents/dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md?ref=master',
    'https://github.com/GSFSGroup/Zinc.Templates/blob/master/dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md',
    'system',
    '5/21/2018'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Zinc.SampleApplicationOne',
    2,
    'adr-0002-organize-solution-into-layers',
    'https://api.github.com/repos/GSFSGroup/Zinc.Templates/contents/dotnet-5.0/docs/RedLine/adr-0002-organize-solution-into-layers.md?ref=master',
    'https://github.com/GSFSGroup/Zinc.Templates/blob/master/dotnet-5.0/docs/RedLine/adr-0002-organize-solution-into-layers.md',
    'system',
    '6/19/2019'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Zinc.SampleApplicationOne',
    3,
    'adr-0003-implement-outbox-on-web-requests',
    'https://api.github.com/repos/GSFSGroup/Zinc.Templates/contents/dotnet-5.0/docs/RedLine/adr-0003-implement-outbox-on-web-requests.md?ref=master',
    'https://github.com/GSFSGroup/Zinc.Templates/blob/master/dotnet-5.0/docs/RedLine/adr-0003-implement-outbox-on-web-requests.md',
    'system',
    '7/4/2020'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Zinc.SampleApplicationTwo',
    1,
    'adr-0001-event-sourcing',
    'https://https://api.github.com/repos/GSFSGroup/Molybdenum.Earnings/contents/docs/App/adr-0001-event-sourcing.md?ref=main',
    'https://github.com/GSFSGroup/Molybdenum.Earnings/blob/main/docs/App/adr-0001-event-sourcing.md',
    'system',
    '1/21/2022'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    number,
    title,
    download_url,
    html_url,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Zinc.SampleApplicationTwo',
    2,
    'adr-0002-select-an-infinite-scroll-library',
    'https://https://api.github.com/repos/GSFSGroup/Molybdenum.Earnings/contents/docs/App/adr-0002-select-an-infinite-scroll-library.md?ref=main',
    'https://github.com/GSFSGroup/Molybdenum.Earnings/blob/main/docs/App/adr-0002-select-an-infinite-scroll-library.md',
    'system',
    '2/13/2022'
);
