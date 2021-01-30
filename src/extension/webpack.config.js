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
                { from: 'icon.png', to: '.' },
                { from: 'manifest.json', to: '.' },
                { from: 'settings.html', to: '.' },
                { from: 'popup.html', to: '.' },
                { from: 'styles.css', to: '.' },

                { from: 'node_modules/bootstrap/dist/js/bootstrap.bundle.min.js', to: './vendor' },
                { from: 'node_modules/bootstrap/dist/css/bootstrap.min.css', to: './vendor' }
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
    },
    externals: {
        mustache: 'Mustache'
    }
};
