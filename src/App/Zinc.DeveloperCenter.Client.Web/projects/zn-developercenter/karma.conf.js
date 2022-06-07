// Karma configuration file, see link for more information
// https://karma-runner.github.io/1.0/config/configuration-file.html

module.exports = function (config) {
    config.set({
        basePath: '',
        frameworks: ['jasmine', '@angular-devkit/build-angular'],
        plugins: [
            require('karma-jasmine'),
            require('karma-chrome-launcher'),
            require('karma-jasmine-html-reporter'),
            require('karma-coverage'),
            require('@angular-devkit/build-angular/plugins/karma')
        ],
        client: {
            clearContext: false // leave Jasmine Spec Runner output visible in browser
        },
        coverageReporter: {
            dir: require('path').join(__dirname, '../../coverage/main'),
            subdir: '.',
            reporters: [
                { type: 'html', subdir: 'report-html' },
                { type: 'lcovonly', file: 'lcov.info' }
            ]
        },
        preprocessors: {
            // source files, that you wanna generate coverage for
            // do not include tests or libraries
            'src/**/!(spec|mock)/*.ts': ['coverage']
        },
        reporters: ['progress', 'kjhtml', 'coverage'],
        port: 9876,
        colors: true,
        logLevel: config.LOG_INFO,
        autoWatch: true,
        browsers: ['ChromeDebugging'],
        singleRun: false,
        restartOnFileChange: true,
        customLaunchers: {
            ChromeDebugging: {
                base: 'Chrome',
                flags: ['--remote-debugging-port=9333']
            },
            ChromeHeadlessCI: {
                base: 'ChromeHeadless',
                flags: ['--no-sandbox']
            }
        }
    });
};
