import path from 'path';
import webpack from 'webpack';

const config: webpack.Configuration = {
    mode: 'development',
    entry: {
        'index': './index.ts',
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].js',
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
};

export default config;
