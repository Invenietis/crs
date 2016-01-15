module.exports = {  
    context: __dirname,
    entry: ['./Ck.Command.ts'],
    output: {
        filename: './dist/ck-command.js',
        library: 'CkCommands',
        libraryTarget: 'var'
    },
    resolve: {
        extensions: ['', '.webpack.js', '.web.js', '.ts', '.js']
    },
    module: {
        loaders: [
            { test: /\.ts(x?)$/, loader: 'ts-loader' }
        ]
    }
}