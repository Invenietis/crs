import path from 'path';
import webpack from 'webpack';

const config: webpack.Configuration = {
    mode: 'development',
    entry: {
        'crs-client': './src/index.ts',
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].js',
        library: 'crsClient',
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
      }
    },
    optimization: {
        splitChunks: {
            chunks: 'all',
        },
    },
};

export default config;
