using NUnit.Framework;

namespace CK.Crs.Tests
{
    [TestFixture]
    public class FeatureTests
    {
        public interface IAmAFeature { }
        public interface IAmAFeature2 { }
        public interface IAmAFeature3 { }

        public class FeatureImpl : IAmAFeature { }
        public class FeatureImpl2 : IAmAFeature2, IAmAFeature3 { }

        [Test]
        public void add_remove_one_feature()
        {
            Feature f = new Feature();
            var feature = f.GetFeature<IAmAFeature>();
            Assert.That( feature, Is.Null );

            f.SetFeature<IAmAFeature>( new FeatureImpl() );
            feature = f.GetFeature<IAmAFeature>();
            Assert.That( feature, Is.Not.Null );

            f.RemoveFeature<IAmAFeature>();
            feature = f.GetFeature<IAmAFeature>();
            Assert.That( feature, Is.Null );

        }


        [Test]
        public void add_remove_two_features()
        {
            Feature f = new Feature();
            var feature = f.GetFeature<IAmAFeature>();
            Assert.That( feature, Is.Null );

            var feature2 = f.GetFeature<IAmAFeature2>();
            Assert.That( feature2, Is.Null );

            f.SetFeature<IAmAFeature>( new FeatureImpl() );
            feature = f.GetFeature<IAmAFeature>();
            Assert.That( feature, Is.Not.Null );

            f.SetFeature<IAmAFeature2>( new FeatureImpl2() );
            feature2 = f.GetFeature<IAmAFeature2>();
            Assert.That( feature2, Is.Not.Null );

            f.RemoveFeature<IAmAFeature>();
            feature = f.GetFeature<IAmAFeature>();
            Assert.That( feature, Is.Null );

            feature2 = f.GetFeature<IAmAFeature2>();
            Assert.That( feature2, Is.Not.Null );
        }

        [Test]
        public void add_remove_three_features()
        {
            Feature f = new Feature();
            var feature = f.GetFeature<IAmAFeature>(); Assert.That( feature, Is.Null );
            var feature2 = f.GetFeature<IAmAFeature2>(); Assert.That( feature2, Is.Null );
            var feature3 = f.GetFeature<IAmAFeature3>(); Assert.That( feature3, Is.Null );

            f.SetFeature<IAmAFeature>( new FeatureImpl() ); feature = f.GetFeature<IAmAFeature>(); Assert.That( feature, Is.Not.Null );
            f.SetFeature<IAmAFeature2>( new FeatureImpl2() ); feature2 = f.GetFeature<IAmAFeature2>(); Assert.That( feature2, Is.Not.Null );
            f.SetFeature<IAmAFeature3>( new FeatureImpl2() ); feature3 = f.GetFeature<IAmAFeature3>(); Assert.That( feature3, Is.Not.Null );

            f.RemoveFeature<IAmAFeature>();
            feature = f.GetFeature<IAmAFeature>(); Assert.That( feature, Is.Null );
            feature2 = f.GetFeature<IAmAFeature2>(); Assert.That( feature2, Is.Not.Null );
            feature3 = f.GetFeature<IAmAFeature3>(); Assert.That( feature3, Is.Not.Null );

            f.SetFeature<IAmAFeature>( new FeatureImpl() ); feature = f.GetFeature<IAmAFeature>(); Assert.That( feature, Is.Not.Null );
            feature = f.GetFeature<IAmAFeature>(); Assert.That( feature, Is.Not.Null );
            feature2 = f.GetFeature<IAmAFeature2>(); Assert.That( feature2, Is.Not.Null );
            feature3 = f.GetFeature<IAmAFeature3>(); Assert.That( feature3, Is.Not.Null );

        }
    }
}
