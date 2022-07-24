INSERT INTO developercenter.application (
    tenant_id,
    name,
    display_name,
    url,
    element,
    description
) VALUES (
    'GSFSGroup',
    'Zinc.Templates',
    'Templates',
    'https://github.com/GSFSGroup/Zinc.Templates',
    'Zinc',
    'Contains dotnet templates for new projects.'
);

INSERT INTO developercenter.application (
    tenant_id,
    name,
    display_name,
    url,
    element,
    description
) VALUES (
    'GSFSGroup',
    'Molybdenum.Earnings',
    'Earnings',
    'https://github.com/GSFSGroup/Molybdenum.Earnings',
    'Molybdenum',
    'A service to calculate earnings based on curves for insurance and insurance-like financial products.'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Zinc.Templates',
    'dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md',
    'Homer Simpson',
    '5/21/2018'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Zinc.Templates',
    'dotnet-5.0/docs/RedLine/adr-0002-organize-solution-into-layers.md',
    'Bart Simpson',
    '6/19/2019'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Zinc.Templates',
    'dotnet-5.0/docs/RedLine/adr-0003-implement-outbox-on-web-requests.md',
    'Lisa Simpson',
    '7/4/2020'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Molybdenum.Earnings',
    'docs/App/adr-0001-event-sourcing.md',
    'Moe Szyslak',
    '1/21/2022'
);

INSERT INTO developercenter.architecture_decision_record (
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    'GSFSGroup',
    'Molybdenum.Earnings',
    'docs/App/adr-0002-select-an-infinite-scroll-library.md',
    'Barney Gumble',
    '2/13/2022'
);
