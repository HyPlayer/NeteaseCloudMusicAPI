using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using NeteaseCloudMusicApi.Utils;
using Newtonsoft.Json;
using static NeteaseCloudMusicApi.CloudMusicApiProvider;

namespace NeteaseCloudMusicApi {
	/// <summary>
	/// 所有网易云音乐API提供者
	/// </summary>
	public static class CloudMusicApiProviders {
		/// <summary>
		///     云盘歌词
		/// </summary>
		public static readonly CloudMusicApiProvider CloudLyric = new CloudMusicApiProvider("/cloud/lyric",
			HttpMethod.Post,
			"https://interface.music.163.com/eapi/cloud/lyric/get",
			new[] {
					  new ParameterInfo("songId") { KeyForwarding = "id" }, new ParameterInfo("userId"),
					  new ParameterInfo("lv", ParameterType.Constant, "0"),
					  new ParameterInfo("kv", ParameterType.Constant, "0"),
					  new ParameterInfo("tv", ParameterType.Constant, "0")
				  }, BuildOptions("eapi", new[] { new Cookie("os", "pc") }, null, "/api/cloud/lyric/get"));


		/// <summary>
		/// 私人 FM
		/// </summary>
		public static readonly CloudMusicApiProvider PersonalFm = new CloudMusicApiProvider("/personal_fm",
			HttpMethod.Post, "https://music.163.com/weapi/v1/radio/get", Array.Empty<ParameterInfo>(),
			BuildOptions("weapi"));

		/// <summary>
		/// 喜欢音乐
		/// </summary>
		public static readonly CloudMusicApiProvider Like = new CloudMusicApiProvider("/like", HttpMethod.Post,
			"https://music.163.com/api/radio/like",
			new[] {
					  new ParameterInfo("alg", ParameterType.Constant, "itembased"),
					  new ParameterInfo("trackId") { KeyForwarding = "id" }, new ParameterInfo("like"),
					  new ParameterInfo("time", ParameterType.Constant, "3")
				  }, BuildOptions("weapi", new[] { new Cookie("os", "pc"), new Cookie("appver", "2.7.1.198277") }));


		/// <summary>
		///     电台 - 详情
		/// </summary>
		public static readonly CloudMusicApiProvider DjDetail = new CloudMusicApiProvider("/dj/detail", HttpMethod.Post,
			"https://music.163.com/weapi/djradio/get",
			new[] { new ParameterInfo("id") { KeyForwarding = "rid" } }, BuildOptions("weapi"));

		/// <summary>
		/// 相关歌单推荐
		/// </summary>
		public static readonly CloudMusicApiProvider RelatedPlaylist = new CloudMusicApiProvider("/related/playlist");

		/// <summary>
		/// 云盘
		/// </summary>
		public static readonly CloudMusicApiProvider UserCloud = new CloudMusicApiProvider("/user/cloud",
			HttpMethod.Post, "https://music.163.com/api/v1/cloud/get",
			new[] {
					  new ParameterInfo("limit", ParameterType.Optional, 30),
					  new ParameterInfo("offset", ParameterType.Optional, 0)
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 删除云盘歌曲
		/// </summary>
		public static readonly CloudMusicApiProvider UserCloudDelete = new CloudMusicApiProvider("/user/cloud/del",
			HttpMethod.Post, "https://music.163.com/weapi/cloud/del",
			new[] {
					  new ParameterInfo("songIds", ParameterType.Required) {
																			   KeyForwarding = "id",
																			   Transformer = JsonArrayTransformer
																		   },
				  }, BuildOptions("weapi", new[] { new Cookie("os", "pc"), new Cookie("appver", "2.7.1.198277") }));

		/// <summary>
		/// 邮箱登录
		/// </summary>
		public static readonly CloudMusicApiProvider Login = new CloudMusicApiProvider("/login", HttpMethod.Post,
			"https://interface.music.163.com/eapi/w/login", new[] {
														   new ParameterInfo("username") { KeyForwarding = "email" },
														   new ParameterInfo("password", ParameterType.Custom) {
															   CustomHandler = q => {
																   if (!q.ContainsKey("md5_password") ||
																	   q.ContainsKey("password")) {
																	   return q["password"].ToString()
																		   .ToByteArrayUtf8().ComputeMd5()
																		   .ToHexStringLower();
																   }

																   return q["md5_password"];
															   }
														   },
														   new ParameterInfo(
															   "remember", ParameterType.Constant, true),
														   new ParameterInfo("type", ParameterType.Constant, 0),
														   new ParameterInfo("https", ParameterType.Constant, true)
													   },
			BuildOptions("weapi", new[] { new Cookie("os", "pc"), new Cookie("appver", "2.9.8") }, "pc"));

		/// <summary>
		/// 音乐是否可用
		/// </summary>
		public static readonly CloudMusicApiProvider CheckMusic = new CloudMusicApiProvider("/check/music",
			HttpMethod.Post, "https://music.163.com/weapi/song/enhance/player/url",
			new[] {
					  new ParameterInfo("ids") { KeyForwarding = "id", Transformer = JsonArrayTransformer },
					  new ParameterInfo("br", ParameterType.Optional, 999000)
				  }, BuildOptions("weapi"));

		/// <summary>
		/// batch批量请求接口
		/// </summary>
		public static readonly CloudMusicApiProvider Batch =
			new CloudMusicApiProvider("/batch", HttpMethod.Post, "http://music.163.com/eapi/batch",
									  Array.Empty<ParameterInfo>(), BuildOptions("eapi", null, null, "/api/batch")) {
				DataProvider = queries => {
					var data = new Dictionary<string, object> { ["e_r"] = true };
					foreach (var query in queries) {
						if (query.Key.StartsWith("/api/", StringComparison.Ordinal))
							data.Add(query.Key, query.Value);
					}

					return data;
				}
			};

		/// <summary>
		/// 新建歌单
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistCreate = new CloudMusicApiProvider("/playlist/create",
			HttpMethod.Post, "https://music.163.com/api/playlist/create",
			new[] {
					  new ParameterInfo("name"), new ParameterInfo("privacy"),
					  new ParameterInfo("type", ParameterType.Optional, "NORMAL"),
				  }, BuildOptions("weapi", new[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 垃圾桶
		/// </summary>
		public static readonly CloudMusicApiProvider FmTrash = new CloudMusicApiProvider("/fm_trash", HttpMethod.Post,
			q =>
				$"https://music.163.com/weapi/radio/trash/add?alg=RT&songId={q["id"]}&time={q.GetValueOrDefault("time", 25)}",
			new[] { new ParameterInfo("songId") { KeyForwarding = "id" } }, BuildOptions("weapi"));

		/// <summary>
		/// 资源点赞( MV,电台,视频)
		/// </summary>
		public static readonly CloudMusicApiProvider ResourceLike = new CloudMusicApiProvider("/resource/like",
			HttpMethod.Post,
			q => $"https://music.163.com/weapi/resource/{(q["t"].ToString() == "1" ? "like" : "unlike")}",
			new[] {
					  new ParameterInfo("threadId", ParameterType.Custom) {
																			  CustomHandler = q =>
																				  q["type"].ToString() == "6"
																					  ? q["threadId"]
																					  : ResourceTypeTransformer(
																						  q["type"]).ToString() +
																					  q["id"]
																		  }
				  }, BuildOptions("weapi", new[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 楼层评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentFloor = new CloudMusicApiProvider("/comment/floor",
			HttpMethod.Post, "https://music.163.com/api/resource/comment/floor/get",
			new[] {
					  new ParameterInfo("threadId", ParameterType.Custom, null) {
						  CustomHandler = q => CommentTypeTransformer(q["type"]) + q["id"].ToString()
					  },
					  new ParameterInfo("parentCommentId"), new ParameterInfo("time", ParameterType.Optional, -1),
					  new ParameterInfo("limit", ParameterType.Optional, 20)
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 给评论点赞
		/// </summary>
		public static readonly CloudMusicApiProvider CommentLike = new CloudMusicApiProvider("/comment/like",
			HttpMethod.Post,
			q => $"https://music.163.com/weapi/v1/comment/{(q["t"].ToString() == "1" ? "like" : "unlike")}",
			new[] {
					  new ParameterInfo("commentId") { KeyForwarding = "cid" },
					  new ParameterInfo("threadId", ParameterType.Custom) {
																			  CustomHandler = q =>
																				  q["type"].ToString() == "6"
																					  ? q["threadId"]
																					  : CommentTypeTransformer(
																						  q["type"]).ToString() +
																					  q["id"]
																		  }
				  }, BuildOptions("weapi", new[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 发送/删除评论
		/// </summary>
		public static readonly CloudMusicApiProvider Comment = new CloudMusicApiProvider("/comment", HttpMethod.Post,
																   q =>
																	   $"https://music.163.com/weapi/resource/comments/{(q["t"].ToString() == "1" ? "add" : (q["t"].ToString() == "0" ? "delete" : "reply"))}",
																   Array.Empty<ParameterInfo>(),
																   BuildOptions(
																	   "weapi", new[] { new Cookie("os", "pc") })) {
			DataProvider = queries => {
				var data = new Dictionary<string, object> {
					["threadId"] =
						CommentTypeTransformer(
								queries["type"])
							.ToString() +
						queries["id"]
				};
				switch (queries["t"]) {
				case "0":
					data.Add("commentId",
						queries["commentId"]);
					break;
				case "1":
					data.Add("content", queries["content"]);
					break;
				case "2":
					data.Add("commentId",
						queries["commentId"]);
					data.Add("content", queries["content"]);
					break;
				default:
					throw new ArgumentOutOfRangeException(
						"t");
				}

				return data;
			}
		};

		/// <summary>
		/// 手机登录
		/// </summary>
		public static readonly CloudMusicApiProvider LoginCellphone = new CloudMusicApiProvider("/login/cellphone",
			HttpMethod.Post, "https://interface.music.163.com/eapi/w/login/cellphone", new[] {
																				new ParameterInfo("phone"),
																				new ParameterInfo(
																					"countrycode",
																					ParameterType.Optional,
																					string.Empty),
																				new ParameterInfo(
																					"password",
																					ParameterType.Custom) {
																					CustomHandler = q => {
																						if (!q.ContainsKey(
																							 "md5_password") ||
																						 q.ContainsKey(
																							 "password")) {
																							return q["password"]
																								.ToString()
																								.ToByteArrayUtf8()
																								.ComputeMd5()
																								.ToHexStringLower();
																						}

																						return q[
																							"md5_password"];
																					}
																				},
																				new ParameterInfo(
																					"rememberLogin",
																					ParameterType.Constant, true),
																				new ParameterInfo("type", ParameterType.Constant, 1),
																				new ParameterInfo("https", ParameterType.Constant, true)
																			},
			BuildOptions("eapi", null, null, "/api/w/login/cellphone"));

		/// <summary>
		/// 签到
		/// </summary>
		public static readonly CloudMusicApiProvider DailySignin = new CloudMusicApiProvider("/daily_signin",
			HttpMethod.Post, "https://music.163.com/weapi/point/dailyTask",
			new[] { new ParameterInfo("type", ParameterType.Optional, 0) }, BuildOptions("weapi"));

		/// <summary>
		/// 听歌打卡
		/// </summary>
		public static readonly CloudMusicApiProvider Scrobble = new CloudMusicApiProvider("/scrobble", HttpMethod.Post,
			"https://music.163.com/weapi/feedback/weblog",
			new[] {
					  new ParameterInfo("logs", ParameterType.Custom) {
																		  CustomHandler = q =>
																			  JsonConvert.SerializeObject(
																				  new[] {
																					  new
																					  Dictionary<string, object> {
																						  ["action"] = "play",
																						  ["json"] =
																							  new Dictionary<
																								  string,
																								  object> {
																								  ["id"] =
																									  q[
																										  "id"],
																								  ["sourceId"] =
																									  q[
																										  "sourceId"],
																								  ["time"] =
																									  q[
																										  "time"],
																								  ["download"] =
																									  0,
																								  ["end"] =
																									  "playend",
																								  ["type"] =
																									  "song",
																								  ["wifi"] =
																									  0,
																								  ["source"] =
																									  "list",
																								  ["mainsite"] =
																									  1,
																								  ["content"] = ""
																							  }
																					  }
																				  })
																	  }
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 二维码校验
		/// </summary>
		public static readonly CloudMusicApiProvider LoginQrCheck = new CloudMusicApiProvider("/login/qr/check",
			HttpMethod.Post, "https://interface.music.163.com/eapi/login/qrcode/client/login",
			new[] { new ParameterInfo("key"), new ParameterInfo("type", ParameterType.Constant, 3) },
			BuildOptions("eapi", null, null, "/api/login/qrcode/client/login"));

		/// <summary>
		/// 热搜列表(简略)
		/// </summary>
		public static readonly CloudMusicApiProvider SearchHot = new CloudMusicApiProvider("/search/hot",
			HttpMethod.Post, "https://music.163.com/weapi/search/hot",
			new[] { new ParameterInfo("type", ParameterType.Constant, 1111) },
			BuildOptions("weapi", null, "mobile"));

		/// <summary>
		/// 新版评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentNew = new CloudMusicApiProvider("/comment/new",
			HttpMethod.Post, "https://music.163.com/api/v2/resource/comments", new[] {
																				   new ParameterInfo(
																					   "threadId",
																					   ParameterType.Custom,
																					   null) {
																					   CustomHandler = q =>
																						   CommentTypeTransformer(
																							   q["type"]) +
																						   q["id"].ToString()
																				   },
																				   new ParameterInfo(
																					   "cursor",
																					   ParameterType.Custom,
																					   null) {
																					   CustomHandler = q =>
																						   q["sortType"]
																							   .ToString() ==
																						   "3"
																							   ? (q
																								   .GetValueOrDefault(
																									   "cursor",
																									   0))
																							   : (q[
																										   "sortType"]
																									   .ToString() ==
																								   "2"
																									   ? "normalHot#"
																									   : "") +
																							   ((int.Parse(
																										   q[
																												   "pageNo"]
																											   .ToString()) -
																									   1) *
																								   int.Parse(
																									   q[
																											   "pageSize"]
																										   .ToString()))
																							   .ToString()
																				   }, //这边三目表达式用了很多,然而int转码可能会报错 毕竟js也会报错
				                                                                   new ParameterInfo(
																					   "pageNo",
																					   ParameterType.Optional, 1),
																				   new ParameterInfo(
																					   "pageSize",
																					   ParameterType.Optional, 20),
																				   new ParameterInfo(
																					   "showInner",
																					   ParameterType.Optional,
																					   true),
																				   new ParameterInfo(
																					   "sortType",
																					   ParameterType.Optional,
																					   1), //1:按推荐排序,2:按热度排序,3:按时间排序
			                                                                   },
			BuildOptions("eapi", new[] { new Cookie("os", "pc") }, null, "/api/v2/resource/comments"));

		/// <summary>
		/// 每日推荐歌单
		/// </summary>
		public static readonly CloudMusicApiProvider RecommendResource =
			new CloudMusicApiProvider("/recommend/resource", HttpMethod.Post,
									  "https://music.163.com/weapi/v1/discovery/recommend/resource",
									  Array.Empty<ParameterInfo>(),
									  BuildOptions("weapi"));

		/// <summary>
		/// 获取歌单详情
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistDetail = new CloudMusicApiProvider("/playlist/detail",
			HttpMethod.Post, "https://music.163.com/api/v6/playlist/detail",
			new[] {
					  new ParameterInfo("id"), new ParameterInfo("n", ParameterType.Constant, 100000),
					  new ParameterInfo("s", ParameterType.Optional, 8)
				  }, BuildOptions("linuxapi"));

		public static readonly CloudMusicApiProvider MlogRcmdFeedList = new CloudMusicApiProvider(
			"/mlog/rcmd/feed/list",
			HttpMethod.Post,
			"https://interface.music.163.com/eapi/mlog/rcmd/feed/list",
			new[] {
					  new ParameterInfo("id", ParameterType.Required),
					  new ParameterInfo("type", ParameterType.Constant, 2),
					  new ParameterInfo("rcmdType", ParameterType.Constant, 20),
					  new ParameterInfo("limit", ParameterType.Optional, 10),
					  new ParameterInfo("extInfo", ParameterType.Custom) {
																			 CustomHandler = (objects => {
																				 if (objects.ContainsKey("songid"))
																					 return "{\"songId\":\"" +
																						 objects["songid"] + "\"}";
																				 return "";
																			 })
																		 }
				  }, BuildOptions("eapi", null, null, "/api/mlog/rcmd/feed/list")
		);

		/// <summary>
		/// 获取 mv 数据
		/// </summary>
		public static readonly CloudMusicApiProvider MvDetail = new CloudMusicApiProvider("/mv/detail", HttpMethod.Post,
			"https://music.163.com/api/v1/mv/detail",
			new[] { new ParameterInfo("id") { KeyForwarding = "mvid" } }, BuildOptions("weapi"));

		/// <summary>
		/// 退出登录
		/// </summary>
		public static readonly CloudMusicApiProvider Logout = new CloudMusicApiProvider("/logout", HttpMethod.Post,
			"https://music.163.com/weapi/logout", Array.Empty<ParameterInfo>(), BuildOptions("weapi", null, "pc"));

		/// <summary>
		/// 收藏的歌手列表
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistSublist = new CloudMusicApiProvider("/artist/sublist",
			HttpMethod.Post, "https://music.163.com/weapi/artist/sublist",
			new[] {
					  new ParameterInfo("limit", ParameterType.Optional, 25),
					  new ParameterInfo("offset", ParameterType.Optional, 0),
					  new ParameterInfo("total", ParameterType.Constant, true)
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 电台的订阅列表
		/// </summary>
		public static readonly CloudMusicApiProvider DjSublist = new CloudMusicApiProvider("/dj/sublist",
			HttpMethod.Post, "https://music.163.com/weapi/djradio/get/subed",
			new[] {
					  new ParameterInfo("limit", ParameterType.Optional, 30),
					  new ParameterInfo("offset", ParameterType.Optional, 0),
					  new ParameterInfo("total", ParameterType.Constant, true)
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 获取用户详情
		/// </summary>
		public static readonly CloudMusicApiProvider UserDetail = new CloudMusicApiProvider("/user/detail",
			HttpMethod.Post, q => $"https://music.163.com/weapi/v1/user/detail/{q["uid"]}",
			Array.Empty<ParameterInfo>(), BuildOptions("weapi"));


		/// <summary>
		/// 获取mlog播放地址
		/// </summary>
		public static readonly CloudMusicApiProvider MlogUrl = new CloudMusicApiProvider("/mlog/url", HttpMethod.Post,
			"https://interface3.music.163.com/eapi/mlog/video/url",
			new[] {
					  new ParameterInfo("mlogIds", ParameterType.Custom) {
																			 CustomHandler =
																				 (objects => "[\"" + objects["id"] +
																					 "\"]"),
																			 KeyForwarding = "id"
																		 },
					  new ParameterInfo("scene", ParameterType.Constant, 0),
					  new ParameterInfo("resolution", ParameterType.Optional, "1080") { KeyForwarding = "res" },
					  new ParameterInfo("type", ParameterType.Constant, 1),
					  new ParameterInfo("netstate", ParameterType.Constant, 1)
				  }, BuildOptions("eapi", null, null, "/api/mlog/video/url"));


		/// <summary>
		/// mv 地址
		/// </summary>
		public static readonly CloudMusicApiProvider MvUrl = new CloudMusicApiProvider("/mv/url", HttpMethod.Post,
			"https://music.163.com/weapi/song/enhance/play/mv/url",
			new[] { new ParameterInfo("id"), new ParameterInfo("r", ParameterType.Optional, 1080) },
			BuildOptions("weapi"));

		/// <summary>
		///     心动模式/智能播放
		/// </summary>
		public static readonly CloudMusicApiProvider PlaymodeIntelligenceList = new CloudMusicApiProvider(
			"/playmode/intelligence/list", HttpMethod.Post,
			"https://interface.music.163.com/eapi/playmode/intelligence/list",
			new[] {
					  new ParameterInfo("songId") { KeyForwarding = "id" },
					  new ParameterInfo("playlistId") { KeyForwarding = "pid" },
					  new ParameterInfo("type", ParameterType.Constant, "fromPlayAll"),
					  new ParameterInfo("startMusicId", ParameterType.Custom) {
																				  CustomHandler = q =>
																					  q.TryGetValue(
																						  "sid", out object sid)
																						  ? sid
																						  : q["id"]
																			  },
					  new ParameterInfo("count", ParameterType.Optional, 1)
				  }, BuildOptions("eapi", null, null, "/api/playmode/intelligence/list"));

		/// <summary>
		/// 所有榜单介绍
		/// </summary>
		public static readonly CloudMusicApiProvider Toplist = new CloudMusicApiProvider("/toplist", HttpMethod.Post,
			q => "https://music.163.com/api/toplist", Array.Empty<ParameterInfo>(), BuildOptions("linuxapi"));

		/// <summary>
		/// 每日推荐歌曲
		/// </summary>
		public static readonly CloudMusicApiProvider RecommendSongs = new CloudMusicApiProvider("/recommend/songs",
			HttpMethod.Post, "https://music.163.com/api/v3/discovery/recommend/songs", Array.Empty<ParameterInfo>(),
			BuildOptions("weapi", new[] { new Cookie("os", "ios") }));

		/// <summary>
		/// 收藏/取消收藏歌单
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistSubscribe = new CloudMusicApiProvider(
			"/playlist/subscribe", HttpMethod.Post,
			q => $"https://music.163.com/weapi/playlist/{(q["t"].ToString() == "1" ? "subscribe" : "unsubscribe")}",
			new[] { new ParameterInfo("id") }, BuildOptions("weapi"));

		/// <summary>
		/// 更新歌单描述
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistDescUpdate = new CloudMusicApiProvider(
			"/playlist/desc/update", HttpMethod.Post, "https://interface3.music.163.com/eapi/playlist/desc/update",
			new[] { new ParameterInfo("id"), new ParameterInfo("desc") },
			BuildOptions("eapi", null, null, "/api/playlist/desc/update"));

		/// <summary>
		/// 云盘 - 搜索
		/// </summary>
		public static readonly CloudMusicApiProvider Cloudsearch = new CloudMusicApiProvider("/cloudsearch",
			HttpMethod.Post, "https://interface.music.163.com/eapi/cloudsearch/pc",
			new[] {
					  new ParameterInfo("s") { KeyForwarding = "keywords" },
					  new ParameterInfo("type", ParameterType.Optional, 1),
					  new ParameterInfo("limit", ParameterType.Optional, 30),
					  new ParameterInfo("offset", ParameterType.Optional, 0),
					  new ParameterInfo("total", ParameterType.Constant, true)
				  }, BuildOptions("eapi", null, null, "/api/cloudsearch/pc"));

		/// <summary>
		/// 已收藏专辑列表
		/// </summary>
		public static readonly CloudMusicApiProvider AlbumSublist = new CloudMusicApiProvider("/album/sublist",
			HttpMethod.Post, "https://music.163.com/weapi/album/sublist",
			new[] {
					  new ParameterInfo("limit", ParameterType.Optional, 25),
					  new ParameterInfo("offset", ParameterType.Optional, 0),
					  new ParameterInfo("total", ParameterType.Constant, true)
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 专辑动态信息
		/// </summary>
		public static readonly CloudMusicApiProvider AlbumDetailDynamic = new CloudMusicApiProvider(
			"/album/detail/dynamic",
			HttpMethod.Post, "https://music.163.com/api/album/detail/dynamic",
			new[] { new ParameterInfo("id") }, BuildOptions("weapi"));

		/// <summary>
		/// 收藏 / 取消收藏专辑
		/// </summary>
		public static readonly CloudMusicApiProvider AlbumSubscribe = new CloudMusicApiProvider("/album/subscribe",
			HttpMethod.Post,
			q => $"https://music.163.com/api/album/{(q["t"].ToString() == "1" ? "sub" : "unsub")}",
			new[] { new ParameterInfo("id") }, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 节目
		/// </summary>
		public static readonly CloudMusicApiProvider DjProgram = new CloudMusicApiProvider("/dj/program",
			HttpMethod.Post, "https://music.163.com/weapi/dj/program/byradio",
			new[] {
					  new ParameterInfo("radioId") { KeyForwarding = "rid" },
					  new ParameterInfo("limit", ParameterType.Optional, 30),
					  new ParameterInfo("offset", ParameterType.Optional, 0),
					  new ParameterInfo("asc", ParameterType.Optional, "false")
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 新碟上架
		/// </summary>
		public static readonly CloudMusicApiProvider TopAlbum = new CloudMusicApiProvider("/top/album", HttpMethod.Post,
			"https://music.163.com/api/discovery/new/albums/area",
			new[] {
					  new ParameterInfo("area", ParameterType.Optional, "ALL"),
					  new ParameterInfo("limit", ParameterType.Optional, 50),
					  new ParameterInfo("offset", ParameterType.Optional, 0),
					  new ParameterInfo("type", ParameterType.Optional, "new"),
					  new ParameterInfo("year", ParameterType.Optional, DateTime.Now.Year.ToString()),
					  new ParameterInfo("month", ParameterType.Optional, (DateTime.Now.Month + 1).ToString()),
					  new ParameterInfo("total", ParameterType.Constant, "false"),
					  new ParameterInfo("rcmd", ParameterType.Constant, true)
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 获取用户播放记录
		/// </summary>
		public static readonly CloudMusicApiProvider UserRecord = new CloudMusicApiProvider("/user/record",
			HttpMethod.Post, "https://music.163.com/weapi/v1/play/record",
			new[] { new ParameterInfo("uid"), new ParameterInfo("type", ParameterType.Optional, 0) },
			BuildOptions("weapi"));

		/// <summary>
		/// 搜索建议
		/// </summary>
		public static readonly CloudMusicApiProvider SearchSuggest = new CloudMusicApiProvider("/search/suggest",
			HttpMethod.Post,
			q =>
				$"https://music.163.com/weapi/search/suggest/{(q.GetValueOrDefault("type", null).ToString() == "mobile" ? "keyword" : "web")}",
			new[] { new ParameterInfo("s") { KeyForwarding = "keywords" } }, BuildOptions("weapi"));

		/// <summary>
		/// 二维码key 获取
		/// </summary>
		public static readonly CloudMusicApiProvider LoginQrKey = new CloudMusicApiProvider("/login/qr/key",
			HttpMethod.Post, "https://interface.music.163.com/eapi/login/qrcode/unikey",
			new[] { new ParameterInfo("type", ParameterType.Constant, 3) }, BuildOptions("eapi", null, null, "/api/login/qrcode/unikey"));


		/// <summary>
		/// 获取用户歌单
		/// </summary>
		public static readonly CloudMusicApiProvider UserPlaylist = new CloudMusicApiProvider("/user/playlist",
			HttpMethod.Post, "https://music.163.com/api/user/playlist",
			new[] {
					  new ParameterInfo("uid"), new ParameterInfo("limit", ParameterType.Optional, 30),
					  new ParameterInfo("includeVideo", ParameterType.Constant, true),
					  new ParameterInfo("offset", ParameterType.Optional, 0)
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 喜欢音乐列表
		/// </summary>
		public static readonly CloudMusicApiProvider Likelist = new CloudMusicApiProvider("/likelist", HttpMethod.Post,
			"https://music.163.com/weapi/song/like/get", new[] { new ParameterInfo("uid") },
			BuildOptions("weapi"));

		/// <summary>
		/// 歌手专辑列表
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistAlbum = new CloudMusicApiProvider("/artist/album",
			HttpMethod.Post, q => $"https://music.163.com/weapi/artist/albums/{q["id"]}",
			new[] {
					  new ParameterInfo("limit", ParameterType.Optional, 30),
					  new ParameterInfo("offset", ParameterType.Optional, 0),
					  new ParameterInfo("total", ParameterType.Constant, "total")
				  }, BuildOptions("weapi"));

		/// <summary>
		/// 歌手详细信息
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistDetail = new CloudMusicApiProvider("/artist/detail",
			HttpMethod.Post, q => "https://music.163.com/api/artist/head/info/get",
			new[] { new ParameterInfo("id") }, BuildOptions("weapi"));

		/// <summary>
		/// 登录状态
		/// </summary>
		public static readonly CloudMusicApiProvider LoginStatus = new CloudMusicApiProvider("/login/status",
			HttpMethod.Post, "https://music.163.com/weapi/w/nuser/account/get", Array.Empty<ParameterInfo>(),
			BuildOptions("weapi"));

		/// <summary>
		/// 获取歌手歌曲
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistSongs = new CloudMusicApiProvider("/artist/songs",
			HttpMethod.Post, "https://music.163.com/api/v1/artist/songs", new[] {
																			  new ParameterInfo("id"),
																			  new ParameterInfo(
																				  "private_cloud",
																				  ParameterType.Constant, true),
																			  new ParameterInfo(
																				  "work_type",
																				  ParameterType.Constant, 1),
																			  new ParameterInfo(
																				  "order", ParameterType.Optional,
																				  "hot"), //hot,time
				                                                              new ParameterInfo(
																				  "offset", ParameterType.Optional,
																				  0), //hot,time
				                                                              new ParameterInfo(
																				  "limit", ParameterType.Optional,
																				  100), //hot,time
			                                                              },
			BuildOptions("weapi", new[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 歌手热门50首歌曲
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistTopSong = new CloudMusicApiProvider("/artist/top/song",
			HttpMethod.Post, "https://music.163.com/api/artist/top/song", new[] { new ParameterInfo("id") },
			BuildOptions("weapi"));

		/// <summary>
		/// 歌词
		/// </summary>
		public static readonly CloudMusicApiProvider Lyric = new CloudMusicApiProvider("/lyric", HttpMethod.Post,
			"https://music.163.com/api/song/lyric?_nmclfl=1",
			new[] {
					  new ParameterInfo("id"), new ParameterInfo("lv", ParameterType.Constant, "-1"),
					  new ParameterInfo("kv", ParameterType.Constant, "-1"),
					  new ParameterInfo("rv", ParameterType.Constant, "-1"),
					  new ParameterInfo("tv", ParameterType.Constant, "-1")
				  }, BuildOptions("linuxapi", new[] { new Cookie("os", "ios") }));

		/// <summary>
		/// 新版歌词
		/// </summary>
		public static readonly CloudMusicApiProvider LyricNew = new CloudMusicApiProvider("/lyric/new", HttpMethod.Post,
			"https://interface3.music.163.com/eapi/song/lyric/v1",
			new[] {
					  new ParameterInfo("id"), new ParameterInfo("cp", ParameterType.Constant, false),
					  new ParameterInfo("tv", ParameterType.Constant, "0"),
					  new ParameterInfo("lv", ParameterType.Constant, "0"),
					  new ParameterInfo("rv", ParameterType.Constant, "0"),
					  new ParameterInfo("kv", ParameterType.Constant, "0"),
					  new ParameterInfo("yv", ParameterType.Constant, "0"),
					  new ParameterInfo("ytv", ParameterType.Constant, "0"),
					  new ParameterInfo("yrv", ParameterType.Constant, "0"),
				  }, BuildOptions("eapi", null, null, "/api/song/lyric/v1"));

		/// <summary>
		/// 获取专辑内容
		/// </summary>
		public static readonly CloudMusicApiProvider Album = new CloudMusicApiProvider("/album", HttpMethod.Post,
			q => $"https://music.163.com/weapi/v1/album/{q["id"]}", Array.Empty<ParameterInfo>(),
			BuildOptions("weapi"));

		/// <summary>
		/// 对歌单添加或删除歌曲
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistTracks = new CloudMusicApiProvider("/playlist/tracks",
			HttpMethod.Post, "https://music.163.com/weapi/playlist/manipulate/tracks",
			new[] {
					  new ParameterInfo("op"), new ParameterInfo("pid"),
					  new ParameterInfo("imme", ParameterType.Constant, true),
					  new ParameterInfo("trackIds") { KeyForwarding = "tracks", Transformer = JsonArrayTransformer }
				  }, BuildOptions("weapi"));


		/// <summary>
		/// 获取音乐 url
		/// </summary>
		public static readonly CloudMusicApiProvider SongUrl = new CloudMusicApiProvider("/song/url", HttpMethod.Post,
			"https://interface3.music.163.com/eapi/song/enhance/player/url",
			new[] {
					  new ParameterInfo("ids") { KeyForwarding = "id", Transformer = JsonArrayTransformer },
					  new ParameterInfo("br", ParameterType.Optional, 999000)
				  },
			BuildOptions("eapi",
						 new[] {
								   new Cookie("os", "pc"),
								   new Cookie("_ntes_nuid", new Random().RandomBytes(16).ToHexStringLower())
							   }, null, "/api/song/enhance/player/url"));


		/// <summary>
		/// 获取音乐 url - v1
		/// </summary>
		public static readonly CloudMusicApiProvider SongUrlV1 = new CloudMusicApiProvider(
			"/song/url/v1", HttpMethod.Post,
			"https://interface.music.163.com/eapi/song/enhance/player/url/v1",
			new[] {
					  new ParameterInfo("ids") { KeyForwarding = "id", Transformer = JsonArrayTransformer },
					  new ParameterInfo("level", ParameterType.Optional, "exhigh"),
					  new ParameterInfo("encodeType", ParameterType.Constant, "flac"),
					  new ParameterInfo("immerseType", ParameterType.Constant, "c51"),
				  },
			BuildOptions("eapi",
						 new[] { new Cookie("os", "android"), new Cookie("appver", "8.10.05") }, null,
						 "/api/song/enhance/player/url/v1"));


		/// <summary>
		/// 获取歌曲详情
		/// </summary>
		public static readonly CloudMusicApiProvider SongDetail = new CloudMusicApiProvider("/song/detail",
			HttpMethod.Post, "https://music.163.com/weapi/v3/song/detail",
			new[] {
					  new ParameterInfo("c") {
												 KeyForwarding = "ids",
												 Transformer = t =>
													 "[" + string.Join(
														 ",",
														 t.ToString().Split(',')
														  .Select(m => "{\"id\":" + m.Trim() + "}")) + "]"
											 },
					  new ParameterInfo("ids") { Transformer = JsonArrayTransformer }
				  }, BuildOptions("weapi"));


		public static readonly CloudMusicApiProvider CloudPub = new CloudMusicApiProvider("/cloud/pub", HttpMethod.Post,
			"https://interface.music.163.com/api/cloud/pub/v2",
			new[] { new ParameterInfo("songid", ParameterType.Required) },
			BuildOptions("weapi", new[] { new Cookie("os", "pc"), new Cookie("appver", "2.7.1.198277") }));

		public static readonly CloudMusicApiProvider CloudUploadToken = new CloudMusicApiProvider("/cloud/upload/token",
			HttpMethod.Post, "https://music.163.com/weapi/nos/token/alloc",
			new[] {
					  new ParameterInfo("bucket", ParameterType.Constant, "jd-musicrep-privatecloud-audio-public"),
					  new ParameterInfo("ext", ParameterType.Optional, "mp3"),
					  new ParameterInfo("filename", ParameterType.Custom) {
																			  CustomHandler = (r) =>
																				  Path.GetFileNameWithoutExtension(
																					  r["filename"].ToString())
																		  },
					  new ParameterInfo("local", ParameterType.Constant, false),
					  new ParameterInfo("nos_product", ParameterType.Constant, 3),
					  new ParameterInfo("type", ParameterType.Constant, "audio"),
					  new ParameterInfo("md5", ParameterType.Required),
				  }, BuildOptions("weapi", new[] { new Cookie("os", "pc"), new Cookie("appver", "2.7.1.198277") }));

		public static readonly CloudMusicApiProvider CloudUploadCheck = new CloudMusicApiProvider("/cloud/upload/check",
			HttpMethod.Post, "https://interface.music.163.com/api/cloud/upload/check",
			new[] {
					  new ParameterInfo("bitrate", ParameterType.Optional, "999000"),
					  new ParameterInfo("ext", ParameterType.Constant, ""),
					  new ParameterInfo("length", ParameterType.Required) { KeyForwarding = "size" },
					  new ParameterInfo("md5", ParameterType.Required),
					  new ParameterInfo("songId", ParameterType.Constant, "0"),
					  new ParameterInfo("version", ParameterType.Constant, 1)
				  }, BuildOptions("weapi", new[] { new Cookie("os", "pc"), new Cookie("appver", "2.7.1.198277") }));

		public static readonly CloudMusicApiProvider UploadCloudInfo = new CloudMusicApiProvider("/upload/cloud/info",
			HttpMethod.Post, "https://music.163.com/api/upload/cloud/info/v2",
			new[] {
					  new ParameterInfo("md5", ParameterType.Required),
					  new ParameterInfo("songid", ParameterType.Required) { KeyForwarding = "songId" },
					  new ParameterInfo("filename", ParameterType.Required),
					  new ParameterInfo("song", ParameterType.Required),
					  new ParameterInfo("album", ParameterType.Optional, "未知专辑"),
					  new ParameterInfo("artist", ParameterType.Optional, "未知艺术家"),
					  new ParameterInfo("bitrate", ParameterType.Required),
					  new ParameterInfo("resourceId", ParameterType.Required)
				  }, BuildOptions("weapi", new[] { new Cookie("os", "pc"), new Cookie("appver", "2.7.1.198277") }));


		/// <summary>
		/// 歌单公开
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistPrivacy = new CloudMusicApiProvider(
			"/playlist/privacy", HttpMethod.Post, "https://interface.music.163.com/eapi/playlist/update/privacy",
			new[] { new ParameterInfo("id"), new ParameterInfo("privacy", ParameterType.Constant, 0) },
			BuildOptions("eapi", null, null, "/api/playlist/update/privacy"));

		/// <summary>
		/// 音乐百科
		/// </summary>
		public static readonly CloudMusicApiProvider SongWikiSummary = new CloudMusicApiProvider(
			"/song/wiki/summary", HttpMethod.Post, "https://interface3.music.163.com/eapi/music/wiki/home/song/get",
			new[] { new ParameterInfo("songId") { KeyForwarding = "id" } },
			BuildOptions("eapi", null, null, "/api/song/play/about/block/page"));


		/// <summary>
		/// 私人 DJ
		/// </summary>
		public static readonly CloudMusicApiProvider AiDjContent = new CloudMusicApiProvider(
			"/aidj/content/rcmd", HttpMethod.Post, "https://interface3.music.163.com/eapi/aidj/content/rcmd/info",
			new[] {
					  new ParameterInfo("extInfo", ParameterType.Custom) {
																			 CustomHandler = (_) => {
																				 return $"{{\"noAidjToAidj\":false,\"lastRequestTimestamp\":{DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()},\"listenedTs\":false}}}}";
																			 }
																		 }
				  }, BuildOptions("eapi", null, null, "/api/aidj/content/rcmd/info"));

		/// <summary>
		///     删除歌单
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistDelete = new CloudMusicApiProvider("/playlist/delete",
			HttpMethod.Post, "https://music.163.com/weapi/playlist/remove",
			new[] { new ParameterInfo("ids") { Transformer = JsonArrayTransformer } },
			BuildOptions("weapi", new[] { new Cookie("os", "pc") }));

		private static Options BuildOptions(string crypto) {
			return BuildOptions(crypto, null);
		}

		private static Options BuildOptions(string crypto, IEnumerable<Cookie> cookies) {
			return BuildOptions(crypto, cookies, null);
		}

		private static Options BuildOptions(string crypto, IEnumerable<Cookie> cookies, string ua) {
			return BuildOptions(crypto, cookies, ua, null);
		}

		private static Options BuildOptions(string crypto, IEnumerable<Cookie> cookies, string ua, string url) {
			var cookies2 = new CookieCollection();
			if (!(cookies is null)) {
				foreach (var cookie in cookies)
					cookies2.Add(cookie);
			}

			var options = new Options { Crypto = crypto, Cookie = cookies2, UA = ua, Url = url };
			return options;
		}

		private static string VideoItemTransformer(object value) {
			string ret = "[";
			foreach (string item in value.ToString().Split(',')) {
				ret += "{ type: 3,id: " + item + "},";
			}

			return ret.TrimEnd(',') + "]";
		}

		private static object ArtistListInitialTransformer(object value) {
			if (value is null)
				return null;
			if (value is string s)
				return (int)char.ToUpperInvariant(s[0]);
			if (value is char c)
				return (int)char.ToUpperInvariant(c);
			var typeCode = Type.GetTypeCode(value.GetType());
			if (TypeCode.SByte <= typeCode && typeCode <= TypeCode.UInt64)
				return value;
			throw new ArgumentOutOfRangeException(nameof(value));
		}

		private static object JsonArrayTransformer(object value) {
			return "[" + (value is string s ? s.Replace(" ", string.Empty) : value) + "]";
		}

		private static string JsonArrayTransformer2(object value) {
			return "[\"" + value.ToString().Replace(" ", string.Empty) + "\"]";
		}

		private static object BannerTypeTransformer(object type) {
			switch (type.ToString()) {
			case "0": return "pc";
			case "1": return "android";
			case "2": return "iphone";
			case "3": return "ipad";
			default: throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private static object CommentTypeTransformer(object type) {
			switch (type.ToString()) {
			case "0": return "R_SO_4_";      // 歌曲
			case "1": return "R_MV_5_";      // MV
			case "2": return "A_PL_0_";      // 歌单
			case "3": return "R_AL_3_";      // 专辑
			case "4": return "A_DJ_1_";      // 电台
			case "5": return "R_VI_62_";     // 视频
			case "6": return "A_EV_2_";      // 动态
			case "7": return "R_MLOG_1001_"; //MLog
			default: throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private static object DjToplistTypeTransformer(object type) {
			switch (type.ToString()) {
			case "new": return 0;
			case "hot": return 1;
			default:
				throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private static object ResourceTypeTransformer(object type) {
			switch (type.ToString()) {
			case "1": return "R_MV_5_";  // MV
			case "4": return "A_DJ_1_";  // 电台
			case "5": return "R_VI_62_"; // 视频
			case "6": return "A_EV_2_";  // 动态
			default: throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private static object TopListIdTransformer(object idx) {
			switch (idx.ToString()) {
			case "0": return 3779629;    // 云音乐新歌榜
			case "1": return 3778678;    // 云音乐热歌榜
			case "2": return 2884035;    // 云音乐原创榜
			case "3": return 19723756;   // 云音乐飙升榜
			case "4": return 10520166;   // 云音乐电音榜
			case "5": return 180106;     // UK排行榜周榜
			case "6": return 60198;      // 美国Billboard周榜
			case "7": return 21845217;   // KTV嗨榜
			case "8": return 11641012;   // iTunes榜
			case "9": return 120001;     // Hit FM Top榜
			case "10": return 60131;      // 日本Oricon周榜
			case "11": return 3733003;    // 韩国Melon排行榜周榜
			case "12": return 60255;      // 韩国Mnet排行榜周榜
			case "13": return 46772709;   // 韩国Melon原声周榜
			case "14": return 112504;     // 中国TOP排行榜(港台榜)
			case "15": return 64016;      // 中国TOP排行榜(内地榜)
			case "16": return 10169002;   // 香港电台中文歌曲龙虎榜
			case "17": return 4395559;    // 华语金曲榜
			case "18": return 1899724;    // 中国嘻哈榜
			case "19": return 27135204;   // 法国 NRJ EuroHot 30周榜
			case "20": return 112463;     // 台湾Hito排行榜
			case "21": return 3812895;    // Beatport全球电子舞曲榜
			case "22": return 71385702;   // 云音乐ACG音乐榜
			case "23": return 991319590;  // 云音乐说唱榜
			case "24": return 71384707;   // 云音乐古典音乐榜
			case "25": return 1978921795; // 云音乐电音榜
			case "26": return 2250011882; // 抖音排行榜
			case "27": return 2617766278; // 新声榜
			case "28": return 745956260;  // 云音乐韩语榜
			case "29": return 2023401535; // 英国Q杂志中文版周榜
			case "30": return 2006508653; // 电竞音乐榜
			case "31": return 2809513713; // 云音乐欧美热歌榜
			case "32": return 2809577409; // 云音乐欧美新歌榜
			case "33": return 2847251561; // 说唱TOP榜
			case "34": return 3001835560; // 云音乐ACG动画榜
			case "35": return 3001795926; // 云音乐ACG游戏榜
			case "36": return 3001890046; // 云音乐ACG VOCALOID榜
			default: throw new ArgumentOutOfRangeException(nameof(idx));
			}
		}
	}
}
