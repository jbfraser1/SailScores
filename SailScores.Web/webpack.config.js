/// <binding BeforeBuild='Run - Development' />
var path = require('path');

module.exports = {
    devtool: "inline-source-map",
    entry: {
        raceEditor: './Scripts/raceEditor'
    },
    output: {
        publicPath: "/js/",
        path: path.join(__dirname, '/wwwroot/js/'),
        filename: '[name].build.js',
        library: 'SailScores'
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                loader: 'ts-loader',
                exclude: /node_modules/
            },
        ]
    },
    resolve: {
        extensions: [".tsx", ".ts", ".js"]
    }
};
