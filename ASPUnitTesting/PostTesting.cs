using Backend_portafolio.Controllers;
using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ASPUnitTesting
{
	public class PostTesting
	{
		private readonly ApiController _apiController;
		private readonly IConfiguration _configuration;
		private readonly IRepositoryCategorias _repositoryCateogorias;
		private readonly IRepositoryPosts _repositoryPosts;
		private readonly IRepositoryLink _repositoryLink;
		private readonly IRepositoryMedia _repositoryMedia;
		private readonly IRepositoryFormat _repositoryFormat;

		public PostTesting()
        {
			_configuration = ConfigurationHelper.GetConfiguration();

			_repositoryCateogorias = new RepositoryCategorias(_configuration);
			_repositoryPosts = new RepositoryPosts(_configuration);
			_repositoryLink = new RepositoryLink(_configuration);
			_repositoryMedia = new RepositoryMedia(_configuration);
			_repositoryFormat = new RepositoryFormat(_configuration);

			_apiController = new ApiController(_repositoryCateogorias, _repositoryPosts, _repositoryLink, _repositoryMedia, _repositoryFormat);
		}

        [Fact]
		public async void Get_Ok()
		{
			// Tu código de prueba aquí
			var result = await _apiController.Entrada();
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async void Get_Quantity()
		{
			var result = (OkObjectResult) await _apiController.Entrada();
			var posts = Assert.IsType<List<PostApiModel>>(result.Value);
			Assert.True(posts.Count() > 0);
		}

		[Fact]
		public async void GetById_Ok()
		{
			int id = 1;
			var result = await _apiController.Entrada(id);
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async void GetById_Quantity()
		{
			int id = 1;
			var result = (OkObjectResult)await _apiController.Entrada(id);
			var post = Assert.IsType<PostApiModel>(result.Value);
			Assert.True(post is not null);
		}

		[Fact]
		public async void getById_Exists()
		{
			int id = 1;
			var result = (OkObjectResult)await _apiController.Entrada(id);
			var post = Assert.IsType<PostApiModel>(result?.Value);

			Assert.True(post is not null);
			Assert.Equal(post?.id, id);
		}

		[Fact]
		public async void getById_NotFound()
		{
			int id = 10000;
			var result = await _apiController.Entrada(id);
			Assert.IsType<NotFoundResult>(result);
		}


	}
}