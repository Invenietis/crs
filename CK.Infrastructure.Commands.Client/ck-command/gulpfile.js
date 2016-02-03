var gulp = require('gulp'),
    concat = require('gulp-concat'),
    ts = require('gulp-typescript'),
    tsProject = ts.createProject('./src/tsconfig.json'),
    webpack = require('webpack-stream');

gulp.task('concat:bundle',['bundle', 'bundle:definitions'], function(){
    return gulp.src(['./dist/ck-command.js', './bundling/ck-command.footer.js'])
        .pipe(concat('ck-command.js'))
        .pipe(gulp.dest('./dist'))
});

gulp.task('compile', function(){
    var tsResult = tsProject.src() // instead of gulp.src(...) 
		.pipe(ts(tsProject));
	
	return tsResult.js.pipe(gulp.dest('./src'));
});

gulp.task('bundle:definitions', function(){
    return gulp.src('./bundling/ck-command.d.ts')
            .pipe(gulp.dest('./dist'));
});

gulp.task('bundle', function(){
    return gulp.src(['./src/Command.ts', './src/AjaxSender.ts','./src/CommandEmitter.ts', './src/SignalRListener.ts'])
            .pipe(webpack(require('./src/webpack.config.js')))
            .pipe(gulp.dest('./dist'));
});
gulp.task('build', ['compile', 'concat:bundle']);