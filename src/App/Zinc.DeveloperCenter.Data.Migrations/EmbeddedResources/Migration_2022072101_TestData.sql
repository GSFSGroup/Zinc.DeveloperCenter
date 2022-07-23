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
    'Zinc',
    'A service to calculate earnings based on curves for insurance and insurance-like financial products.'
);

INSERT INTO developercenter.architecture_decision_record (
    id,
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    '9f1840ec-03de-43ee-bcde-270596eb0f82',
    'GSFSGroup',
    'Zinc.Templates',
    'dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md',
    'Homer Simpson',
    '5/21/2018'
);

INSERT INTO developercenter.architecture_decision_record (
    id,
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    '241dfb9a-3231-4a1b-9bf4-b1b7035970fc',
    'GSFSGroup',
    'Zinc.Templates',
    'dotnet-5.0/docs/RedLine/adr-0002-organize-solution-into-layers.md',
    'Bart Simpson',
    '6/19/2019'
);

INSERT INTO developercenter.architecture_decision_record (
    id,
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    '3f59ec32-7c6e-49ad-822b-1159e33fba37',
    'GSFSGroup',
    'Zinc.Templates',
    'dotnet-5.0/docs/RedLine/adr-0003-implement-outbox-on-web-requests.md',
    'Lisa Simpson',
    '7/4/2020'
);

INSERT INTO developercenter.architecture_decision_record (
    id,
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    '718cbd4e-4828-418c-8ed5-30a87e5be9b7',
    'GSFSGroup',
    'Molybdenum.Earnings',
    'docs/App/adr-0001-event-sourcing.md',
    'Moe Szyslak',
    '1/21/2022'
);

INSERT INTO developercenter.architecture_decision_record (
    id,
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    '55db5b8f-a425-4271-9ae9-6262c9fabf40',
    'GSFSGroup',
    'Molybdenum.Earnings',
    'docs/App/adr-0002-select-an-infinite-scroll-library.md',
    'Barney Gumble',
    '2/13/2022'
);
