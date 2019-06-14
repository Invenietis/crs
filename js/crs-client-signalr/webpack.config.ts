import path from 'path';
import webpack from 'webpack';

const config: webpack.Configuration = {
    mode: 'development',
    entry: './src/index.ts',
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'crs-client-signalr.js',
        library: 'crsClientSignalr',
        libraryTarget: 'umd'
    },
    resolve: {
      extensions: ['.ts', '.tsx', '.js', '.jsx']
    },
    devtool: 'source-map',
    module: {
      rules: [
        {
          test: /\.tsx?$/,
          loader: 'ts-loader'
        }
      ]
    },
    externals: {
      '@aspnet/signalr': {
        commonjs: 'signalR',
        commonjs2: 'signalR',
        amd: 'signalR',
        root: 'signalR'
      }
    }
};

export default config;