const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const CopyWebpackPlugin = require("copy-webpack-plugin");

module.exports = {
    output: {
        filename: '[name].js',
        path: __dirname + '/dist',
        library: 'LinxChromeExtension'
    },
    entry: {
        'content': './content.ts',
        'settings': './settings.ts',
        'popup': './popup.ts'
    },
    devtool: 'inline-source-map',
    plugins: [
        new CleanWebpackPlugin(),
        new CopyWebpackPlugin({
            patterns: [
                { from: 'favicon-16x16.png', to: '.' },
                { from: 'favicon-32x32.png', to: '.' },
                { from: 'icon-192x192.png', to: '.' },
                { from: 'icon-512x512.png', to: '.' },

                { from: 'manifest.json', to: '.' },
                { from: 'settings.html', to: '.' },
                { from: 'popup.html', to: '.' },
                { from: 'styles.css', to: '.' },

                { from: 'node_modules/bootstrap/dist/js/bootstrap.bundle.min.js', to: './vendor' },
                { from: 'node_modules/bootstrap/dist/css/bootstrap.min.css', to: './vendor' },
                { from: 'node_modules/bootstrap-icons/font/bootstrap-icons.css', to: './vendor' },
                { from: 'node_modules/bootstrap-icons/font/fonts/bootstrap-icons.woff', to: './vendor/fonts/bootstrap-icons.woff' },
                { from: 'node_modules/bootstrap-icons/font/fonts/bootstrap-icons.woff2', to: './vendor/fonts/bootstrap-icons.woff2' }
            ]
        })
    ],
    resolve: {
        extensions: ['.ts', '.tsx', '.js', '.json']
    },
    module: {
        rules: [
            // All files with a '.ts' or '.tsx' extension will be handled by 'ts-loader'
            { test: /\.tsx?$/, exclude: /node_modules/, loader: 'ts-loader' },
            // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'
            { enforce: 'pre', test: /\.js$/, loader: 'source-map-loader' }
        ]
    }
};
