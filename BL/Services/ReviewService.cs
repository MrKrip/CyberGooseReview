using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entity;
using DAL.Interfaces;

namespace BLL.Services
{
    public class ReviewService : IReviewService
    {
        private IUnitOfWork DataBase;
        public ReviewService(IUnitOfWork db)
        {
            DataBase = db;
        }

        public void CreateReview(ReviewDTO review)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewDTO>());
            var mapper = new Mapper(config);
            DataBase.Reviews.Create(mapper.Map<Review>(review));
            DataBase.save();
        }

        public void DeleteReview(int reviewId)
        {
            DataBase.Reviews.Delete(reviewId);
            DataBase.save();
        }

        public void DisLikeReview(int reviewId, int userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReviewDTO> GetAllReviewsToProduct(int productId)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewDTO>());
            var mapper = new Mapper(config);
            return DataBase.Reviews.Find(r => r.ProductId == productId).Select(r => mapper.Map<ReviewDTO>(r));
        }

        public IEnumerable<ReviewDTO> GetAllUserReview(int userId)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewDTO>());
            var mapper = new Mapper(config);
            return DataBase.Reviews.Find(r => r.UserId == userId).Select(r => mapper.Map<ReviewDTO>(r));
        }

        public ReviewDTO GetReview(int id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewDTO>());
            var mapper = new Mapper(config);
            return mapper.Map<ReviewDTO>(DataBase.Reviews.Get(id));
        }

        public void LikeReview(int reviewId, int userId)
        {
            throw new NotImplementedException();
        }

        public void UpdateReview(ReviewDTO review)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewDTO>());
            var mapper = new Mapper(config);
            DataBase.Reviews.Update(mapper.Map<Review>(review));
            DataBase.save();
        }
    }
}
