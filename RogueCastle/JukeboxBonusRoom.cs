/*
  Rogue Legacy Enhanced

  This project is based on modified disassembly of Rogue Legacy's engine, with permission to do so by its creators..
  Therefore, former creators copyright notice applies to original disassembly. 

  Disassembled source Copyright(C) 2011-2015, Cellar Door Games Inc.
  Rogue Legacy(TM) is a trademark or registered trademark of Cellar Door Games Inc. All Rights Reserved.
*/

using System;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using Tweener.Ease;

namespace RogueCastle
{
	public class JukeboxBonusRoom : BonusRoomObj
	{
		private GameObj m_jukeBox;
		private string[] m_songList;
		private string[] m_songTitleList;
		private int m_songIndex;
		private bool m_rotatedLeft;
		private TextObj m_nowPlayingText;
		private TextObj m_songTitle;
		private SpriteObj m_speechBubble;
		public JukeboxBonusRoom()
		{
			m_songList = new string[]
			{
				"CastleBossSong",
				"GardenSong",
				"GardenBossSong",
				"TowerSong",
				"TowerBossSong",
				"DungeonSong",
				"DungeonBoss",
				"CastleSong",
				"PooyanSong",
				"LegacySong",
				"SkillTreeSong",
				"TitleScreenSong",
				"CreditsSong",
				"LastBossSong",
				"EndSong",
				"EndSongDrums"
			};
			m_songTitleList = new string[]
			{
				"Pistol Shrimp",
				"The Grim Outdoors",
				"Skin Off My Teeth",
				"Narwhal",
				"Lamprey",
				"Broadside of the Broadsword",
				"Mincemeat",
				"Trilobyte",
				"Poot-yan",
				"SeaSawHorse (Legacy)",
				"SeaSawHorse (Manor)",
				"Rogue Legacy",
				"The Fish and the Whale",
				"Rotten Legacy",
				"Whale. Shark.",
				"Whale. Shark. (Drums)"
			};
		}
		public override void Initialize()
		{
			m_speechBubble = new SpriteObj("UpArrowSquare_Sprite");
			m_speechBubble.Flip = SpriteEffects.FlipHorizontally;
			m_speechBubble.Visible = false;
			GameObjList.Add(m_speechBubble);
			foreach (GameObj current in GameObjList)
			{
				if (current.Name == "Jukebox")
				{
					m_jukeBox = current;
					break;
				}
			}
			(m_jukeBox as SpriteObj).OutlineWidth = 2;
			m_jukeBox.Y -= 2f;
			m_speechBubble.Position = new Vector2(m_jukeBox.X, m_jukeBox.Y - m_speechBubble.Height - 20f);
			base.Initialize();
		}
		public override void LoadContent(GraphicsDevice graphics)
		{
			m_songTitle = new TextObj(null);
			m_songTitle.Font = Game.JunicodeLargeFont;
			m_songTitle.Align = Types.TextAlign.Right;
			m_songTitle.Text = "Song name here";
			m_songTitle.Opacity = 0f;
			m_songTitle.FontSize = 40f;
			m_songTitle.Position = new Vector2(1270f, 570f);
			m_songTitle.OutlineWidth = 2;
			m_nowPlayingText = (m_songTitle.Clone() as TextObj);
			m_nowPlayingText.Text = "Now Playing";
			m_nowPlayingText.FontSize = 24f;
			m_nowPlayingText.Y -= 50f;
			base.LoadContent(graphics);
		}
		public override void OnEnter()
		{
			m_jukeBox.Scale = new Vector2(3f, 3f);
			m_jukeBox.Rotation = 0f;
			base.OnEnter();
		}
		public override void Update(GameTime gameTime)
		{
			if (CollisionMath.Intersects(Player.Bounds, m_jukeBox.Bounds))
			{
				m_speechBubble.Visible = true;
				m_speechBubble.Y = m_jukeBox.Y - m_speechBubble.Height - 110f + (float)Math.Sin(Game.TotalGameTime * 20f) * 2f;
				if (Game.GlobalInput.JustPressed(16) || Game.GlobalInput.JustPressed(17))
				{
					Tween.StopAllContaining(m_jukeBox, false);
					m_jukeBox.Scale = new Vector2(3f, 3f);
					m_jukeBox.Rotation = 0f;
					Tween.StopAllContaining(m_nowPlayingText, false);
					Tween.StopAllContaining(m_songTitle, false);
					m_songTitle.Opacity = 0f;
					m_nowPlayingText.Opacity = 0f;
					Tween.To(m_songTitle, 0.5f, new Easing(Linear.EaseNone), new string[]
					{
						"delay",
						"0.2",
						"Opacity",
						"1"
					});
					m_songTitle.Opacity = 1f;
					Tween.To(m_songTitle, 0.5f, new Easing(Linear.EaseNone), new string[]
					{
						"delay",
						"2.2",
						"Opacity",
						"0"
					});
					m_songTitle.Opacity = 0f;
					Tween.To(m_nowPlayingText, 0.5f, new Easing(Linear.EaseNone), new string[]
					{
						"Opacity",
						"1"
					});
					m_nowPlayingText.Opacity = 1f;
					Tween.To(m_nowPlayingText, 0.5f, new Easing(Linear.EaseNone), new string[]
					{
						"delay",
						"2",
						"Opacity",
						"0"
					});
					m_nowPlayingText.Opacity = 0f;
					SoundManager.PlayMusic(m_songList[m_songIndex], true, 1f);
					m_songTitle.Text = m_songTitleList[m_songIndex];
					m_songIndex++;
					if (m_songIndex > m_songList.Length - 1)
					{
						m_songIndex = 0;
					}
					AnimateJukebox();
					CheckForSongRepeat();
				}
			}
			else
			{
				m_speechBubble.Visible = false;
			}
			base.Update(gameTime);
		}
		private void CheckForSongRepeat()
		{
			Game.ScreenManager.GetLevelScreen().JukeboxEnabled = true;
		}
		public void AnimateJukebox()
		{
			Tween.To(m_jukeBox, 0.2f, new Easing(Tween.EaseNone), new string[]
			{
				"ScaleY",
				"2.9",
				"ScaleX",
				"3.1",
				"Rotation",
				"0"
			});
			Tween.AddEndHandlerToLastTween(this, "AnimateJukebox2", new object[0]);
			Player.AttachedLevel.ImpactEffectPool.DisplayMusicNote(new Vector2(m_jukeBox.Bounds.Center.X + CDGMath.RandomInt(-20, 20), m_jukeBox.Bounds.Top + CDGMath.RandomInt(0, 20)));
		}
		public void AnimateJukebox2()
		{
			if (!m_rotatedLeft)
			{
				Tween.To(m_jukeBox, 0.2f, new Easing(Tween.EaseNone), new string[]
				{
					"Rotation",
					"-2"
				});
				m_rotatedLeft = true;
			}
			else
			{
				Tween.To(m_jukeBox, 0.2f, new Easing(Tween.EaseNone), new string[]
				{
					"Rotation",
					"2"
				});
				m_rotatedLeft = false;
			}
			Tween.To(m_jukeBox, 0.2f, new Easing(Tween.EaseNone), new string[]
			{
				"ScaleY",
				"3.1",
				"ScaleX",
				"2.9"
			});
			Tween.AddEndHandlerToLastTween(this, "AnimateJukebox", new object[0]);
		}
		public override void Draw(Camera2D camera)
		{
			m_songTitle.Position = new Vector2(X + 1320f - 50f, Y + 720f - 150f);
			m_nowPlayingText.Position = m_songTitle.Position;
			m_nowPlayingText.Y -= 50f;
			base.Draw(camera);
			SamplerState value = camera.GraphicsDevice.SamplerStates[0];
			camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			m_songTitle.Draw(camera);
			m_nowPlayingText.Draw(camera);
			camera.GraphicsDevice.SamplerStates[0] = value;
		}
		public override void Dispose()
		{
			if (!IsDisposed)
			{
				m_songTitle.Dispose();
				m_songTitle = null;
				m_nowPlayingText.Dispose();
				m_nowPlayingText = null;
				m_jukeBox = null;
				Array.Clear(m_songList, 0, m_songList.Length);
				Array.Clear(m_songTitleList, 0, m_songTitleList.Length);
				m_songTitleList = null;
				m_songList = null;
				m_speechBubble = null;
				base.Dispose();
			}
		}
	}
}
