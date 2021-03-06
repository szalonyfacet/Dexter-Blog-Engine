﻿#region Disclaimer/Info

// ////////////////////////////////////////////////////////////////////////////////////////////////
// File:			PostDataService.cs
// Website:		http://dexterblogengine.com/
// Authors:		http://dexterblogengine.com/aboutus
// Created:		2012/11/02
// Last edit:	2013/03/24
// License:		New BSD License (BSD)
// For updated news and information please visit http://dexterblogengine.com/
// Dexter is hosted to Github at https://github.com/imperugo/Dexter-Blog-Engine
// For any question contact info@dexterblogengine.com
// ////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

namespace Dexter.Data.Raven.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using global::AutoMapper;

	using Common.Logging;

	using Dexter.Data.Raven.Domain;
	using Dexter.Data.Raven.Extensions;
	using Dexter.Data.Raven.Helpers;
	using Dexter.Data.Raven.Indexes.Reading;
	using Dexter.Data.Raven.Indexes.Updating;
	using Dexter.Data.Raven.Session;
	using Dexter.Entities;
	using Dexter.Entities.Filters;
	using Dexter.Entities.Result;
	using Dexter.Shared.Exceptions;

	using global::Raven.Client;

	using global::Raven.Client.Linq;

	public class PostDataService : ServiceBase, IPostDataService
	{
		#region Fields

		private readonly IDocumentStore store;

		#endregion

		#region Constructors and Destructors

		public PostDataService(ILog logger, ISessionFactory sessionFactory, IDocumentStore store)
			: base(logger, sessionFactory)
		{
			this.store = store;
		}

		#endregion

		#region Public Methods and Operators

		public void Delete(int id)
		{
			Post post = this.Session
			                .Include<Post>(x => x.CommentsId)
			                .Include<Post>(x => x.TrackbacksId)
			                .Load<Post>(id);

			ItemComments comments = this.Session.Load<ItemComments>(post.CommentsId);
			ItemTrackbacks trackbacks = this.Session.Load<ItemTrackbacks>(post.TrackbacksId);

			if (post == null)
			{
				throw new DexterPostNotFoundException(id);
			}

			this.Session.Delete(post);
			this.Session.Delete(comments);
			this.Session.Delete(trackbacks);
		}

		public IList<MonthDto> GetMonthsForPublishedPosts()
		{
			DateTime now = DateTime.Now;

			List<MonthDto> dates = this.Session.Query<MonthDto, MonthOfPublishedPostsWithCountIndex>()
			                           .OrderByDescending(x => x.Year)
			                           .ThenByDescending(x => x.Month)
			                           .Where(x => (x.Year < now.Year || x.Year == now.Year) && x.Month <= now.Month)
			                           .ToList();

			return dates;
		}

		public PostDto GetPostByKey(int id)
		{
			if (id < 1)
			{
				throw new ArgumentException("The Id must be greater than 0", "id");
			}

			Post post = this.Session
			                .Include<Post>(x => x.CommentsId)
			                .Include<Post>(x => x.TrackbacksId)
			                .Load(id);

			if (post == null)
			{
				return null;
			}

			PostDto result = post.MapTo<PostDto>();

			return result;
		}

		public PostDto GetPostBySlug(string slug)
		{
			Post post = this.GetPostBySlugInternal(slug);

			if (post == null)
			{
				throw new DexterPostNotFoundException(slug);
			}

			PostDto result = post.MapTo<PostDto>();

			return result;
		}

		public IPagedResult<PostDto> GetPosts(int pageIndex, int pageSize, ItemQueryFilter filters)
		{
			if (pageIndex < 1)
			{
				throw new ArgumentException("The page index must be greater than 0", "pageIndex");
			}

			if (pageSize < 1)
			{
				throw new ArgumentException("The page size must be greater than 0", "pageSize");
			}

			RavenQueryStatistics stats;

			return this.Session.Query<Post>()
			           .Statistics(out stats)
			           .ApplyFilterItem(filters)
			           .OrderByDescending(post => post.PublishAt)
			           .ToPagedResult<Post, PostDto>(pageIndex, pageSize, stats);
		}

		public IPagedResult<PostDto> GetPostsByDate(int pageIndex, int pageSize, int year, int? month, int? day, ItemQueryFilter filters)
		{
			if (pageIndex < 1)
			{
				throw new ArgumentException("The page index must be greater than 0", "pageIndex");
			}

			if (pageSize < 1)
			{
				throw new ArgumentException("The page size must be greater than 0", "pageSize");
			}

			if (year < 1700)
			{
				throw new ArgumentException("The year value must be greater than 1700. Internet did not exist in 1700!", "year");
			}

			if (month.HasValue && (month.Value < 1 || month.Value > 12))
			{
				throw new ArgumentException("The month value must be greater than 0 and lesser than 12", "month");
			}

			if (day.HasValue && (day.Value < 1 || day.Value > 31))
			{
				throw new ArgumentException("The day value must be greater than 0 and lesser than 31", "month");
			}

			RavenQueryStatistics stats;

			IQueryable<Post> query = this.Session.Query<Post>()
			                             .Statistics(out stats)
			                             .Where(post => post.PublishAt.Year == year)
			                             .ApplyFilterItem(filters);

			if (month.HasValue)
			{
				query = query.Where(post => post.PublishAt.Month == month.Value);
			}

			if (day.HasValue)
			{
				query = query.Where(post => post.PublishAt.Day == day.Value);
			}

			return query
				.OrderByDescending(post => post.PublishAt)
				.ToPagedResult<Post, PostDto>(pageIndex, pageSize, stats);
		}

		public IPagedResult<PostDto> GetPostsByTag(int pageIndex, int pageSize, string tag, ItemQueryFilter filters)
		{
			if (pageIndex < 1)
			{
				throw new ArgumentException("The page index must be greater than 0", "pageIndex");
			}

			if (pageSize < 1)
			{
				throw new ArgumentException("The page size must be greater than 0", "pageSize");
			}

			if (string.IsNullOrEmpty(tag))
			{
				throw new ArgumentException("The tag must contains a valid value.", "tag");
			}

			RavenQueryStatistics stats;

			return this.Session.Query<Post>()
			           .Statistics(out stats)
			           .ApplyFilterItem(filters)
			           .Where(post => post.Tags.Any(postTag => postTag == tag))
			           .OrderByDescending(post => post.PublishAt)
			           .ToPagedResult<Post, PostDto>(pageIndex, pageSize, stats);
		}

		public IPagedResult<PostDto> GetPostsByCategory(int pageIndex, int pageSize, string categoryName, ItemQueryFilter filters)
		{
			if (pageIndex < 1)
			{
				throw new ArgumentException("The page index must be greater than 0.", "pageIndex");
			}

			if (pageSize < 1)
			{
				throw new ArgumentException("The page size must be greater than 0.", "pageSize");
			}

			if (string.IsNullOrEmpty(categoryName))
			{
				throw new ArgumentException("The category name must contains a valid value.", "categoryName");
			}

			RavenQueryStatistics stats;

			return this.Session.Query<Post>()
					   .Statistics(out stats)
					   .ApplyFilterItem(filters)
					   .Where(post => post.Categories.Any(postCategory => postCategory == categoryName))
					   .OrderByDescending(post => post.PublishAt)
					   .ToPagedResult<Post, PostDto>(pageIndex, pageSize, stats);
		}

		public IList<TagDto> GetTopTagsForPublishedPosts(int numberOfTags)
		{
			if (numberOfTags < 1)
			{
				throw new ArgumentException("The number of tags to retrieve must be greater than 0", "numberOfTags");
			}

			return this.Session.Query<TagDto, TagsForPublishedPostsWithCountIndex>()
			           .OrderBy(x => x.Count)
			           .Take(numberOfTags)
			           .ToList();
		}

		public void SaveOrUpdate(PostDto item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item", "The post must be contains a valid instance");
			}

			Post post = this.Session.Load<Post>(item.Id)
			            ?? new Post
				               {
					               CreatedAt = DateTimeOffset.Now
				               };

			if (string.IsNullOrEmpty(item.Author))
			{
				item.Author = Thread.CurrentPrincipal.Identity.Name;
			}

			item.MapPropertiesToInstance(post);

			if (string.IsNullOrEmpty(post.Excerpt))
			{
				string generateAbstract = AbstractHelper.GenerateAbstract(post.Content);
				post.Excerpt = generateAbstract;
			}

			if (string.IsNullOrEmpty(post.Slug))
			{
				post.Slug = SlugHelper.GenerateSlug(post.Title, post.Id, this.GetPostBySlugInternal);
			}

			if (post.IsTransient)
			{
				ItemComments comments = new ItemComments
					                        {
						                        Item = new ItemReference
							                               {
								                               Id = post.Id, 
								                               Status = post.Status, 
								                               ItemPublishedAt = post.PublishAt
							                               }
					                        };

				this.Session.Store(comments);
				post.CommentsId = comments.Id;

				ItemTrackbacks trackbacks = new ItemTrackbacks
					                            {
						                            Item = new ItemReference
							                                   {
								                                   Id = post.Id, 
								                                   Status = post.Status, 
								                                   ItemPublishedAt = post.PublishAt
							                                   }
					                            };

				this.Session.Store(trackbacks);
				post.TrackbacksId = trackbacks.Id;
			}

			this.Session.Store(post);

			UpdateDenormalizedItemIndex.UpdateIndexes(this.store, this.Session, post);

			item.Id = RavenIdHelper.Resolve(post.Id);
		}

		public void SaveTrackback(TrackBackDto trackBack, int itemId)
		{
			if (trackBack == null)
			{
				throw new ArgumentNullException("trackBack", "The trackBack must be contains a valid instance");
			}

			if (itemId < 1)
			{
				throw new ArgumentException("The Id must be greater than 0", "itemId");
			}

			Post post = this.Session
			                .Include<Post>(x => x.TrackbacksId)
			                .Load<Post>(itemId);

			if (post == null)
			{
				throw new DexterPostNotFoundException(itemId);
			}

			ItemTrackbacks trackbacks = this.Session.Load<ItemTrackbacks>(post.TrackbacksId)
			                            ?? new ItemTrackbacks
				                               {
					                               Item = new ItemReference
						                                      {
							                                      Id = post.Id, 
							                                      Status = post.Status, 
							                                      ItemPublishedAt = post.PublishAt
						                                      }
				                               };

			trackbacks.AddTrackback(trackBack.MapTo<Trackback>(), trackBack.Status);

			this.Session.Store(trackbacks);
			post.TrackbacksId = trackbacks.Id;

			this.Session.Store(post);
		}

		public IPagedResult<PostDto> Search(string term, int pageIndex, int pageSize, ItemQueryFilter filters)
		{
			RavenQueryStatistics stats;

			return this.Session.Query<PostFullTextIndex.ReduceResult, PostFullTextIndex>()
			           .Customize(x => x.Highlight("SearchQuery", 128, 1, "Results"))
			           .Search(x => x.SearchQuery, term)
			           .OrderByDescending(post => post.PublishDate)
			           .Statistics(out stats)
			           .As<Post>()
			           .ApplyFilterItem(filters)
			           .ToPagedResult<Post, PostDto>(pageIndex, pageSize, stats);
		}

		#endregion

		#region Methods

		private Post GetPostBySlugInternal(string slug)
		{
			if (slug == null)
			{
				throw new ArgumentNullException("slug");
			}

			if (slug == string.Empty)
			{
				throw new ArgumentException("The string must have a value.", "slug");
			}

			Post post = this.Session.Query<Post>()
			                .Where(x => x.Slug == slug)
			                .Include(x => x.CommentsId)
			                .Include(x => x.TrackbacksId)
			                .FirstOrDefault();

			return post;
		}

		#endregion
	}
}