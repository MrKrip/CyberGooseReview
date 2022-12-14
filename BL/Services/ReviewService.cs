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
            var user = _userManager.FindByIdAsync(review.UserId).Result;
            if (review.Rating < 1)
            {
                review.Rating = 1;
            }
            else if (review.Rating > 10)
            {
                review.Rating = 10;
            }
            DataBase.Reviews.Create(new Review()
            {
                Product = DataBase.Products.Get(review.ProductId),
                ProductId = review.ProductId,
                CreationDate = DateTime.Now,
                Rating = review.Rating,
                ReviewDetails = review.ReviewDetails,
                UserId = review.UserId,
                IsCritic = DataBase.CategoryRoles.Find(cr => _userManager.IsInRoleAsync(user, _roleManager.FindByIdAsync(cr.RoleID).Result.Name).Result).Any()
            });
            DataBase.save();
            UpdataeRating(review.ProductId);
        }

        public void DeleteReview(int reviewId)
        {
            var ProdId = DataBase.Reviews.Get(reviewId).ProductId;
            DataBase.Reviews.Delete(reviewId);
            DataBase.save();
            UpdataeRating(ProdId);
        }

        public void DisLikeReview(int reviewId, string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReviewDTO> FindUserReviews(Func<Review, bool> predicate)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Product, ProductDTO>();
            });
            var mapper = new Mapper(config);
            return DataBase.Reviews.Find(predicate).Select(r => new ReviewDTO()
            {
                Id = r.Id,
                Product = mapper.Map<ProductDTO>(r.Product),
                CreationDate = r.CreationDate,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                ProductId = r.ProductId,
                Rating = r.Rating,
                ReviewDetails = r.ReviewDetails,
                UserId = r.UserId,
                User = mapper.Map<UserDTO>(_userManager.FindByIdAsync(r.UserId).Result)
            });
        }

        public IEnumerable<ReviewDTO> GetAllReviewsToProduct(int productId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Product, ProductDTO>();
            });
            var mapper = new Mapper(config);
            var result = DataBase.Reviews.Find(r => r.ProductId == productId).Select(r => new ReviewDTO()
            {
                Id = r.Id,
                Product = mapper.Map<ProductDTO>(r.Product),
                CreationDate = r.CreationDate,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                ProductId = r.ProductId,
                Rating = r.Rating,
                ReviewDetails = r.ReviewDetails,
                UserId = r.UserId
            }).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].User = mapper.Map<UserDTO>(_userManager.FindByIdAsync(result[i].UserId).Result);
            }
            return result;
        }

        public IEnumerable<ReviewDTO> GetAllUserReview(string userId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Product, ProductDTO>();
            });
            var mapper = new Mapper(config);
            return DataBase.Reviews.Find(r => r.UserId == userId).ToList().Select(r => new ReviewDTO()
            {
                Id = r.Id,
                Product = mapper.Map<ProductDTO>(r.Product),
                CreationDate = r.CreationDate,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                ProductId = r.ProductId,
                Rating = r.Rating,
                ReviewDetails = r.ReviewDetails,
                UserId = r.UserId,
                User = mapper.Map<UserDTO>(_userManager.FindByIdAsync(r.UserId).Result)
            });
        }

        public ReviewDTO GetReview(int id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Product, ProductDTO>();
            });
            var mapper = new Mapper(config);
            var r = DataBase.Reviews.Get(id);
            return new ReviewDTO()
            {
                Id = r.Id,
                Product = mapper.Map<ProductDTO>(r.Product),
                CreationDate = r.CreationDate,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                ProductId = r.ProductId,
                Rating = r.Rating,
                ReviewDetails = r.ReviewDetails,
                UserId = r.UserId,
                User = mapper.Map<UserDTO>(_userManager.FindByIdAsync(r.UserId).Result)
            };
        }

        public void LikeReview(int reviewId, string userId)
        {
            throw new NotImplementedException();
        }

        public void UpdataeRating(int productId)
        {
            var prod = DataBase.Products.Get(productId);

            var sum = 0;
            var reviews = DataBase.Reviews.Find(r => r.ProductId == productId).ToList();
            if (reviews != null)
            {
                //common rating
                sum = 0;
                foreach (var review in reviews)
                {
                    sum += review.Rating;
                }
                var a = (((float)sum) / reviews.Count) * 10;
                prod.CommonRating = (int)a;

                //Critic rating               
                var criticsReviews = new List<Review>();
                for (int i = 0; i < reviews.Count; i++)
                {
                    var result = IsCritic(reviews[i].UserId, prod.CategoryId);
                    if (result)
                    {
                        criticsReviews.Add(reviews[i]);
                    }
                }
                sum = 0;
                foreach (var review in criticsReviews)
                {
                    sum += review.Rating;
                }
                if (criticsReviews.Count > 0)
                {
                    a = (((float)sum) / criticsReviews.Count) * 10;
                    prod.CriticRating = (int)a;
                }
                else
                {
                    prod.CriticRating = 0;
                }

                //Users Rating
                var usersReviews = new List<Review>();
                sum = 0;
                for (int i = 0; i < reviews.Count; i++)
                {
                    var result = !IsCritic(reviews[i].UserId, prod.CategoryId);
                    if (result)
                    {
                        usersReviews.Add(reviews[i]);
                    }
                }
                foreach (var review in usersReviews)
                {
                    sum += review.Rating;
                }
                if (usersReviews.Count > 0)
                {
                    a = (((float)sum) / usersReviews.Count) * 10;
                    prod.UserRating = (int)a;
                }
                else
                {
                    prod.UserRating = 0;
                }
            }
            else
            {
                prod.CommonRating = 0;
                prod.CriticRating = 0;
                prod.UserRating = 0;
            }
            DataBase.Products.Update(prod);
            DataBase.save();
        }

        public void UpdateReview(ReviewDTO review)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductDTO, Product>());
            var mapper = new Mapper(config);
            DataBase.Reviews.Update(new Review()
            {
                Id = review.Id,
                Product = mapper.Map<Product>(review.Product),
                CreationDate = review.CreationDate,
                ProductId = review.ProductId,
                Rating = review.Rating,
                ReviewDetails = review.ReviewDetails,
                UserId = review.UserId
            });
            DataBase.save();
        }

        public bool IsCritic(string UserId, int CatId)
        {
            var CatRoles = DataBase.CategoryRoles.Find(cr => cr.CategoryId == CatId);
            var user = _userManager.FindByIdAsync(UserId).Result;
            foreach (var role in CatRoles)
            {
                var roleName = _roleManager.FindByIdAsync(role.RoleID).Result.Name;
                if (_userManager.IsInRoleAsync(user, roleName).Result)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
