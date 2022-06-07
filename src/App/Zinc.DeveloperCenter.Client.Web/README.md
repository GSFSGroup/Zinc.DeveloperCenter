
## Angular Applications

### Styling

We are using [Material UI](https://material.angular.io) and not Bootstrap for our UI framework. We strive to follow Material design guidelines.

---

### VS Code extensions you will need

- [EditorConfig for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)This plugin attempts to override user/workspace settings with settings found in .editorconfig files.

- [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode) - Code formatter

- [Prettier ESLint](https://marketplace.visualstudio.com/items?itemName=rvest.vs-code-prettier-eslint) - ECMAScript - Prettier linter.

- [TSLint](https://marketplace.visualstudio.com/items?itemName=ms-vscode.vscode-typescript-tslint-plugin) - Type script linter.

---

#### Installing Node packages

```sh
npm install
```

---

## A few npm tips you can use

Run the scripts defined in `package.json` with:

```sh
npm run scriptname
```

Run another angular cli tool with:
```sh
npx ng [Angular CLI Command]
```

> __TIP:__ `npx` is a npm utility that allows you to run any commands defined by your dependencies. You can see a list of them in `node_modules/.bin` after you install dependencies.

### When starting UI work

#### Start a watch-build

We have created a handy script in `package.json` for this. At the root of the `Zinc.DeveloperCenter.Client.Web`

```sh
npm run watch
```

#### Start the microapp

The watch-build copies the bundles into `Zinc.DeveloperCenter.Host.Web/wwwroot/dist/main` directory, which is under the static file root for the web host. The static file middleware in the web host looks for the angular app in this folder. So, after the build, we need to start the web host to be able to use the UI.

### Testing and linting

#### Starting tests

At the root of the `Zinc.DeveloperCenter.Client.Web`, start the test session with watch:

```sh
npm run test:watch
```

> __TIP:__ For faster feedback loop, name your test description function as `fdescribe` instead of `describe`; which will result in only those tests being run in the session. If you do, do not forget to rename it back to `describe` before you check in.

#### Linting
Show files with linting errors. At the root of the `Zinc.DeveloperCenter.Client.Web`:

```sh
npm run lint
```

#### Lint and Fix
Autofixing the linting errors (will only fix the lint errors which have an autofix defined). At the root of the `Zinc.DeveloperCenter.Client.Web`:

```json
npm run lint:fix
```

---

## Folder Structure
##### References
[Angular Style Guide](https://angular.io/guide/styleguide#application-structure-and-ngmodules)
[How to define a highly scalable folder structure for your Angular project](https://itnext.io/choosing-a-highly-scalable-folder-structure-in-angular-d987de65ec7)

```
|-- app
    |-- modules
        |-- resource-management (module for resource management admin ui)
            |-- (other files for the module)
            |-- resource-management.component.{ts|html|scss|spec} (main component)
            |-- resource-management.module.ts (the module definition)
        |-- * (other modules)
        |-- core (core modules that form the base functionality)
            |-- [+] authentication
            |-- http
                |-- api.service.ts|spec.ts
            |-- [+] errorHandler
            |-- [+] interceptors
            |-- [+] mocks
            |-- [+] services
            |-- [+] header
            |-- core.module.ts
            |-- module-import-guard.ts
    |-- screens (components for UX screens. See adr-0008)
    |-- shared (components, directives, pipes and models shared between all modules.)
            |-- [+] components
            |-- [+] directives
            |-- [+] pipes
            |-- [+] models
    |-- app-routing.module.ts (defines routing rules)
    |-- app.component.{ts,html,spec.ts,scss} (main component)
    |-- app.module.ts (module definition for the main component)
|-- assets (various assets used and get copied to the bundle location)
|-- styles.scss
```

### The Core Module
The CoreModule takes on the role of the root AppModule , but is not the module which gets bootstrapped by Angular at run-time. The CoreModule should contain singleton services (which is usually the case), universal components and other features where there’s only once instance per application. To prevent re-importing the core module elsewhere, you should also add a guard for it in the core module’ constructor. Some examples of itmes to put in the core module are:

- **Authentication:** Handles the authentication cycle of the user
- **ErrorHandler:** Application wide error handler
- **Http:** Holds services which make http request (web api calls)
- **Interceptors:** Allows modification of the http request before it is sent.
- **Services:** All additional singleton services.

### The Shared Module
The SharedModule is where any shared components, pipes/filters and services should go. The SharedModule can be imported in any other module when those items will be re-used. The shared module shouldn’t have any dependency to the rest of the application and should therefore not rely on any other module.

- **components:** The components folder contains all the “shared” components. This are components like loaders and buttons , which multiple components would benefit from.
- **directives:** Contains the directives used across the application.
- **pipes:** Contains the pipes used across the application.
- **models:** Contains the models used across the application.

