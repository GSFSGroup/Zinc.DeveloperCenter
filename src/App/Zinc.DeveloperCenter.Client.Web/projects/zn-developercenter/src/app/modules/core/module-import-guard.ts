// source: https://github.com/johnpapa/angular-first-look-examples/blob/master/_examples/storyline-tracker/app/core/module-import-guard.ts
// Ensures a module is only imported once.

// eslint-disable-next-line prefer-arrow/prefer-arrow-functions
export function throwIfAlreadyLoaded(parentModule: any, moduleName: string) {
    if (parentModule) {
        throw new Error(`${moduleName} has already been loaded. Import Core modules in the AppModule only.`);
    }
}
