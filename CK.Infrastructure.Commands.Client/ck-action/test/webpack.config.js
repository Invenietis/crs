module.exports = {  
    entry: ['./ActionHandler.spec.ts', './Resolver.spec.ts'],
    output: {
        filename: 'specs.js',
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