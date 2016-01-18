var gulp = require('gulp'),
    webpack = require('webpack-stream');

 
gulp.task('build', function(){
    return gulp.src(['./src/Command.ts', './src/AjaxSender.ts','./src/CommandEmitter.ts', './src/SignalRListener.ts'])
            .pipe(webpack(require('./src/webpack.config.js')))
            .pipe(gulp.dest('./dist'));
});