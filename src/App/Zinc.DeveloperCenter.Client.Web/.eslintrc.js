/**
 * -----------------------------------------------------
 * NOTES ON CONFIGURATION STRUCTURE
 * -----------------------------------------------------
 *
 * Out of the box, ESLint does not support TypeScript or HTML. Naturally those are the two
 * main file types we care about in Angular projects, so we have to do a little extra work
 * to configure ESLint exactly how we need to.
 *
 * Fortunately, ESLint gives us an "overrides" configuration option which allows us to set
 * different lint tooling (parser, plugins, rules etc) for different file types, which is
 * critical because our .ts files require a different parser and different rules to our
 * .html (and our inline Component) templates.
 */
 module.exports = {
  "root": true,
  "plugins": [
    "prefer-arrow",
    "eslint-plugin-import",
    "eslint-plugin-jsdoc",
    "@angular-eslint/eslint-plugin",
    "eslint-plugin-react",
    "@typescript-eslint",
    "jasmine"
  ],
  "ignorePatterns": [".eslintrc.js"],
  "overrides": [
    /**
     * -----------------------------------------------------
     * TYPESCRIPT FILES (COMPONENTS, SERVICES ETC) (.ts)
     * -----------------------------------------------------
     */
    {
      "files": ["*.ts"],


      "parser": "@typescript-eslint/parser",
      "parserOptions": {
        "project": "./tsconfig.json",
        "tsconfigRootDir": __dirname,
      },
      "extends": [
        "plugin:@angular-eslint/ng-cli-compat",
        "plugin:jasmine/recommended",
        "plugin:@angular-eslint/recommended",
        "plugin:@typescript-eslint/eslint-recommended",
        "plugin:@angular-eslint/template/process-inline-templates"
      ],

      "rules": {
        /**
         * Any TypeScript related rules you wish to use/reconfigure over and above the
         * recommended set provided by the @angular-eslint project would go here.
         *
         * There are some examples below from the @angular-eslint plugin and ESLint core:
         */
        "@angular-eslint/directive-selector": ["error", { "type": "attribute", "prefix": "app", "style": "camelCase" }],
        "@angular-eslint/component-selector": ["error", { "type": "element", "prefix": "app", "style": "kebab-case" }],
        "@angular-eslint/no-outputs-metadata-property": "error",
        "@angular-eslint/use-lifecycle-interface": "error",
        "@angular-eslint/use-pipe-transform-interface": "error",
        "@typescript-eslint/consistent-type-definitions": "error",
        "@typescript-eslint/explicit-member-accessibility": "error",
        "@typescript-eslint/indent": [
          "error",
          4,
          {
            "ArrayExpression": "first",
            "ObjectExpression": "first",
            "FunctionDeclaration": {
              "parameters": "first"
            },
            "FunctionExpression": {
              "parameters": "first"
            }
          }
        ],
        "@typescript-eslint/member-delimiter-style": [
          "error",
          {
            "multiline": {
              "delimiter": "semi",
              "requireLast": true
            },
            "singleline": {
              "delimiter": "semi",
              "requireLast": false
            }
          }
        ],
        "@typescript-eslint/member-ordering": [
          "error",
          // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/member-ordering.md
          {
            "default": [
              "signature",
              ["field", "get", "set"],
              "constructor",
              "method"
            ]
          }
        ],
        "no-underscore-dangle": "off",
        "@typescript-eslint/naming-convention": [
          "error",
          {
            "selector": ["variable", "function"],
            "format": ["camelCase"],
            "leadingUnderscore": "allow"
          }
        ],
        "@typescript-eslint/no-empty-function": "off",
        "@typescript-eslint/no-empty-interface": "error",
        "@typescript-eslint/no-inferrable-types": [
          "error",
          {
            "ignoreParameters": true
          }
        ],
        "@typescript-eslint/no-misused-new": "error",
        /* non-null assertions are necessary when using obversables with typescript's strict checks */
        "@typescript-eslint/no-non-null-assertion": "off",
        "no-shadow": "off",
        "@typescript-eslint/no-shadow": "error",

        /**
          The following 3 lines have to do with using the type "any"
        **/
        "@typescript-eslint/no-unsafe-member-access": "off",
        "@typescript-eslint/no-unsafe-assignment": "off",
        "@typescript-eslint/no-unsafe-return": "off",

        "@typescript-eslint/no-unused-expressions": "error",
        "no-unused-vars": "off",
        "@typescript-eslint/no-unused-vars": "error",
        "@typescript-eslint/prefer-function-type": "error",
        "@typescript-eslint/quotes": [
          "error",
          "single",
          {
            "avoidEscape": true,
            "allowTemplateLiterals": true
          }
        ],
        "@typescript-eslint/semi": ["error", "always"],

        "@typescript-eslint/type-annotation-spacing": "error",
        "@typescript-eslint/unified-signatures": "error",

        "arrow-body-style": "error",
        "arrow-parens": ["error", "as-needed"],
        "brace-style": [
          "error",
          "1tbs",
          {
            "allowSingleLine": false
          }
        ],
        "comma-dangle": "error",
        "constructor-super": "error",
        "curly": "error",
        "eol-last": "error",
        "eqeqeq": ["error", "smart"],
        "guard-for-in": "error",
        "id-blacklist": [
          "error",
          "any",
          "Number",
          "number",
          "String",
          "string",
          "Boolean",
          "boolean",
          "Undefined",
          "undefined"
        ],
        "id-match": "error",
        "import/no-deprecated": "warn",
        "import/order": ["error",
          {
            "groups": ["builtin", "external", "internal", ["parent", "sibling"], "index"],
            "newlines-between": "always",
            "alphabetize": {
              "order": "asc", /* sort in ascending order. Options: ['ignore', 'asc', 'desc'] */
              "caseInsensitive": true /* ignore case. Options: [true, false] */
            },
            "pathGroups": [{
              "pattern": "~/**",
              "group": "internal"
            }]
          }],
        "jsdoc/no-types": "error",
        "linebreak-style": "off",
        "max-len": [
          "off",
          {
            "code": 140
          }
        ],
        "new-parens": "error",
        "newline-per-chained-call": "off",
        "no-bitwise": "error",
        "no-caller": "error",
        "no-console": [
          "error",
          {
            "allow": [
              "log",
              "warn",
              "dir",
              "timeLog",
              "assert",
              "clear",
              "count",
              "countReset",
              "group",
              "groupEnd",
              "table",
              "dirxml",
              "error",
              "groupCollapsed",
              "Console",
              "profile",
              "profileEnd",
              "timeStamp",
              "context"
            ]
          }
        ],
        "no-constant-condition": "error",
        "no-control-regex": "error",
        "no-debugger": "error",
        "no-empty": "off",
        "no-eval": "error",
        "no-extra-semi": "error",
        "no-fallthrough": "error",
        "no-invalid-regexp": "error",
        "no-irregular-whitespace": "error",
        "no-multiple-empty-lines": "off",
        "no-new-wrappers": "error",
        "no-obj-calls": "error",
        "no-regex-spaces": "error",
        "no-restricted-imports": ["error", "rxjs/Rx"],
        "no-throw-literal": "error",
        "no-trailing-spaces": "error",
        "no-undef-init": "error",
        "no-unused-labels": "error",
        "no-var": "error",
        "object-shorthand": ["error", "consistent"],
        "padding-line-between-statements": [
          "off",
          {
            "blankLine": "always",
            "prev": "*",
            "next": "return"
          }
        ],
        "prefer-arrow-callback": [
          "error",
          { "allowNamedFunctions": true }
        ],
        "prefer-arrow/prefer-arrow-functions": [
          "warn",
          {
            "allowStandaloneDeclarations": true,
            "disallowPrototype": true,
            "singleReturnOnly": false,
            "classPropertiesAllowed": false
          }
        ],
        "prefer-const": "error",
        "quote-props": "off",
        "quotes": ["error", "single", { "allowTemplateLiterals": true }],
        "radix": "error",
        "react/jsx-curly-spacing": "off",
        "react/jsx-equals-spacing": "off",
        "react/jsx-wrap-multilines": "off",
        "space-before-function-paren": "off",
        "space-in-parens": ["off", "never"],
        "spaced-comment": [
          "error",
          "always",
          {
            "markers": ["/"]
          }
        ],
        "use-isnan": "error"
      }
    },

    {
      "files": ["*.spec.ts", "test.ts"],
      "parser": "@typescript-eslint/parser",
      "parserOptions": {
        "project": "./tsconfig.json",
        "tsconfigRootDir": __dirname,
      },
      "rules": {
        "@typescript-eslint/no-floating-promises": "off",
        "@typescript-eslint/no-unsafe-assignment": "off",
        "@typescript-eslint/no-unsafe-call": "off",
        "@typescript-eslint/no-unsafe-member-access": "off",
        "@typescript-eslint/unbound-method": "off",
        "@typescript-eslint/no-misused-promises": "off",
        "@typescript-eslint/member-ordering": "off",
        "jasmine/prefer-toHaveBeenCalledWith": "off",
        "@typescript-eslint/no-non-null-assertion": "off"
      }
    },

    /**
     * -----------------------------------------------------
     * COMPONENT TEMPLATES
     * -----------------------------------------------------
     *
     * If you use inline templates, make sure you read the notes on the configuration
     * object after this one to understand how they relate to this configuration directly
     * below.
     */
    {
      "files": ["*.component.html"],
      "parser": "@angular-eslint/template-parser",
      "parserOptions": {
        "project": "./tsconfig.json",
        "tsconfigRootDir": __dirname,
      },
      "extends": ["plugin:@angular-eslint/template/recommended"],
      "rules": {
      }
    },

    /**
     * -----------------------------------------------------
     * EXTRACT INLINE TEMPLATES (from within .component.ts)
     * -----------------------------------------------------
     *
     * This extra piece of configuration is necessary to extract inline
     * templates from within Component metadata, e.g.:
     *
     * @Component({
     *  template: `<h1>Hello, World!</h1>`
     * })
     * ...
     *
     * It works by extracting the template part of the file and treating it as
     * if it were a full .html file, and it will therefore match the configuration
     * specific for *.component.html above when it comes to actual rules etc.
     *
     * NOTE: This processor will skip a lot of work when it runs if you don"t use
     * inline templates in your projects currently, so there is no great benefit
     * in removing it, but you can if you want to.
     *
     * You won"t specify any rules here. As noted above, the rules that are relevant
     * to inline templates are the same as the ones defined for *.component.html.
     */
    {
      "files": ["*.component.ts"],
      "extends": ["plugin:@angular-eslint/template/process-inline-templates"]
    }
  ]
}
