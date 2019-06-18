import path from 'path';
import webpack from 'webpack';

const config: webpack.Configuration = {
    mode: 'development',
    entry: {
        'crs-client-signalr': './src/index.ts',
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].js',
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
      axios: {
          commonjs: 'axios',
          commonjs2: 'axios',
          amd: 'axios',
          root: 'axios'
      },
      '@aspnet/signalr': {
        commonjs: 'signalR',
        commonjs2: 'signalR',
        amd: 'signalR',
        root: 'signalR'
      }
    },
    optimization: {
        splitChunks: {
            chunks: 'all',
        },
    },
};

export default config;
