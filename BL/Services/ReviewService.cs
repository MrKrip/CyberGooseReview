using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entity;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public class ReviewService : IReviewService
    {
        private IUnitOfWork DataBase;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ReviewService(IUnitOfWork db, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            DataBase = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void CreateReview(ReviewDTO review)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ReviewDTO, Review>());
            var mapper = new Mapper(config);
            DataBase.Reviews.Create(mapper.Map<Review>(review));
            DataBase.save();
        }

        public void DeleteReview(int reviewId)
        {
            DataBase.Reviews.Delete(reviewId);
            DataBase.save();
        }

        public void DisLikeReview(int reviewId, string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReviewDTO> FindUserReviews(Func<Review, bool> predicate)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ReviewDTO, Review>());
            var mapper = new Mapper(config);
            return DataBase.Reviews.Find(predicate).Select(r => mapper.Map<ReviewDTO>(r));
        }

        public IEnumerable<ReviewDTO> GetAllReviewsToProduct(int productId)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewDTO>());
            var mapper = new Mapper(config);
            return DataBase.Reviews.Find(r => r.ProductId == productId).Select(r => mapper.Map<ReviewDTO>(r));
        }

        public IEnumerable<ReviewDTO> GetAllUserReview(string userId)
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

        public void LikeReview(int reviewId, string userId)
        {
            throw new NotImplementedException();
        }

        public void UpdataeRating(int productId)
        {
            throw new NotImplementedException();
        }

        public void UpdateReview(ReviewDTO review)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ReviewDTO, Review>());
            var mapper = new Mapper(config);
            DataBase.Reviews.Update(mapper.Map<Review>(review));
            DataBase.save();
        }
    }
}
