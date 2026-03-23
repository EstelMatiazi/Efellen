using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using System.Collections.Generic;
using Server.Misc;
using System.Collections;
using System.Text;
using System.IO;
using Server.Regions;
using Server.Targeting;

namespace Server.Items
{
	public class HiddenTrap : Item
	{
		public override bool DisplayWeight { get { return false; } }

		public int HiddenTrapType;

		[CommandProperty(AccessLevel.Owner)]
		public int Hidden_TrapType { get { return HiddenTrapType; } set { HiddenTrapType = value; InvalidateProperties(); } }

		[Constructable]
		public HiddenTrap() : base( 0x65F7 )
		{
			Movable = false;
			Name = "a hidden trap";
			Visible = false;
			Weight = 1.0;
			Light = LightType.Circle150;

			// Weight Values:
			// 1.0 = Hidden and unverified
			// 2.0 = Hidden and active
			// 3.0 = Visible and active
			// 5.0 = Deactivated
			// 6.0 = Remove due to unverification
		}

        public override void OnAfterSpawn()
        {
			base.OnAfterSpawn();
			SetAppearance( this );
		}

		public static void SetAppearance( Item trap )
		{
			if ( trap.Weight >= 5.0 )
				trap.ItemID = 0x65FB;
			else
				trap.ItemID = 0x65F7;

			if ( trap.Weight == 5.0 )
				trap.Name = "a disabled trap";
			else if ( trap.Weight == 6.0 )
				trap.Name = "a broken trap";
			else if ( trap.Weight == 3.0 )
				trap.Name = "a trap";
		}

		public HiddenTrap(Serial serial) : base(serial)
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			string sTrapType = "";

			string textSay = "";
			string textLog = "";

			if ( !SeeIfTrapActive( this ) )
				return true;

			if ( !CanSetOffTraps( m ) || Weight >= 5.0 )
				return true;

			if ( PassiveSearching( this, m ) )
				return true;

			bool HadAnyAffect = false;

			if ( m is PlayerMobile )
			{
				bool nSprung = CheckTrapAvoidance( m, this );

				if ( nSprung )
				{
					int nTrapType = Utility.RandomMinMax( 1, 25 );

					if ( HiddenTrapType > 0 ){ nTrapType = HiddenTrapType; }

					HiddenTrapType = nTrapType;

					if ( m is PlayerMobile && Spells.Research.ResearchAirWalk.UnderEffect( m ) )
					{
						Point3D air = new Point3D( ( m.X+1 ), ( m.Y+1 ), ( m.Z+5 ) );
						Effects.SendLocationParticles(EffectItem.Create(air, m.Map, EffectItem.DefaultDuration), 0x2007, 9, 32, Server.Misc.PlayerSettings.GetMySpellHue( true, m, 0 ), 0, 5022, 0);
						m.PlaySound( 0x014 );
					}
					else if ( nTrapType == 1 && SavingThrow( m, "Magic", true, this ) == false ) // REVEALING TRAP
					{
						textSay = "Você ativou uma armadilha mágica reveladora!";
						textLog = "a revealing trap";
			
						if ( m.Hidden != false ){ m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay); sTrapType = textLog; }
					}
					else if ( nTrapType == 2 && SavingThrow( m, "Agility", true, this ) == false ) // TRIP WIRE
					{
						int HowBad = Utility.RandomMinMax( 1, 5 );

						if ( HowBad == 1 )
						{
							textSay = "Você tropeçou em um fio e derrubou sua mochila!";
							textLog = "a trip wire trap";
				
							int nDrop = 0;

							List<Item> belongings = new List<Item>();
							foreach( Item i in m.Backpack.Items )
							{
								belongings.Add(i);
								nDrop = 1;
							}

							if ( nDrop > 0 )
							{
								Container box = new DroppedContainer();
								foreach ( Item stuff in belongings )
								{
									if ( stuff != null && stuff.LootType != LootType.Blessed )
										box.DropItem(stuff);
								}
								box.MoveToWorld( this.Location, this.Map );
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
								m.PlaySound( m.Female ? 812 : 1086 );
								sTrapType = textLog;
							}
						}
						else
						{
							textSay = "Você tropeçou em um fio e derrubou um de seus itens equipados!";
							textLog = "a trip wire trap";
				
							Item iTripped = GetMyItem( m );

							if ( iTripped != null )
							{
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
								m.PlaySound( m.Female ? 812 : 1086 );
								iTripped.MoveToWorld( this.Location, this.Map );
								sTrapType = textLog;
							}
						}
					}
					else if ( nTrapType == 3 && SavingThrow( m, "Magic", true, this ) == false ) // COINS TO LEAD TRAP
					{
						textSay = "Uma armadilha foi ativada, deixando suas moedas mais pesadas!";
						textLog = "a transmutation trap";
			
						Container cont = m.Backpack;
						int nDull = 0;

						int m_gAmount = m.Backpack.GetAmount( typeof( Gold ) );
						int m_cAmount = m.Backpack.GetAmount( typeof( DDCopper ) );
						int m_sAmount = m.Backpack.GetAmount( typeof( DDSilver ) );
						int m_xAmount = m.Backpack.GetAmount( typeof( DDXormite ) );

						if ( cont.ConsumeTotal( typeof( Gold ), m_gAmount ) )
						{
							m.AddToBackpack( new LeadCoin( m_gAmount ) );
							nDull = 1;
						}
						if ( cont.ConsumeTotal( typeof( DDCopper ), m_cAmount ) )
						{
							m.AddToBackpack( new LeadCoin( m_cAmount ) );
							nDull = 1;
						}
						if ( cont.ConsumeTotal( typeof( DDSilver ), m_sAmount ) )
						{
							m.AddToBackpack( new LeadCoin( m_sAmount ) );
							nDull = 1;
						}
						if ( cont.ConsumeTotal( typeof( DDXormite ), m_xAmount ) )
						{
							m.AddToBackpack( new LeadCoin( m_xAmount ) );
							nDull = 1;
						}
						if ( nDull > 0 )
						{
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							m.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
							m.PlaySound( 0x1E1 );
						}
						sTrapType = textLog;
					}
					else if ( nTrapType == 4 && SavingThrow( m, "Magic", true, this ) == false ) // LOSE ITEM TRAP
					{
						textSay = "Uma armadilha foi ativada, quase estragando um dos seus itens protegidos!";
						textLog = "a destructive trap";
			

						Container cont = m.Backpack;
						Item iRuined = GetMyItem( m );

						if ( iRuined != null )
						{
							if ( Mobile.InsuranceEnabled && CheckInsuranceOnTrap( iRuined, m ) )
							{
								m.LocalOverheadMessage(MessageType.Emote, 1150, true, textSay);
							}
							else
							{
								textSay = "Uma armadilha foi ativada, enferrujando um dos seus itens equipados!";
					
								int Rusted = 0;
								if ( iRuined is BaseWeapon )
								{
									BaseWeapon iRusted = (BaseWeapon)iRuined;

									if ( CraftResources.GetType( iRuined.Resource ) == CraftResourceType.Metal )
									{
										m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
										RustyJunk broke = new RustyJunk();
										broke.ItemID = iRuined.GraphicID;
										broke.Name = "rusted item";
										broke.Weight = iRuined.Weight;
										m.AddToBackpack ( broke );
										Rusted = 1;
									}
								}
								if ( iRuined is BaseArmor )
								{
									BaseArmor iRusted = (BaseArmor)iRuined;

									if ( CraftResources.GetType( iRuined.Resource ) == CraftResourceType.Metal )
									{
										m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
										RustyJunk broke = new RustyJunk();
										broke.ItemID = iRuined.ItemID;
										broke.Name = "rusted item";
										broke.Weight = Utility.RandomMinMax( 1, 4 );
										m.AddToBackpack ( broke );
										Rusted = 1;
									}
								}
								if ( Rusted == 0 )
								{
									textSay = "Uma armadilha foi ativada, estragando um dos seus itens equipados!";
						
									m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
									BrokenGear broke = new BrokenGear();
									broke.ItemID = iRuined.ItemID;
									broke.Name = "Ruined Item";
									broke.Weight = iRuined.Weight;
									m.AddToBackpack ( broke );
								}
								iRuined.Delete();
							}
							m.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
							m.PlaySound( 0x1E1 );
							sTrapType = textLog;
						}
					}
					else if ( nTrapType == 5 && SavingThrow( m, "Magic", true, this ) == false ) // LOSE A STAT TRAP
					{
						int nStat = Utility.RandomMinMax( 1, 3 );

						if ( nStat == 1 )
						{
							if ( m.RawStr > 10 )
							{
								textSay = "Uma armadilha foi ativada, fazendo você se sentir mais fraco!";
								textLog = "a weakness trap";
					
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
								m.FixedParticles( 0x3779, 10, 15, 5009, EffectLayer.Waist );
								m.PlaySound( 0x1E6 );
								m.RawStr = m.RawStr - 1; 
								sTrapType = textLog;
							}
						}
						else if ( nStat == 2 )
						{
							if ( m.RawDex > 10 )
							{
								textSay = "Uma armadilha foi ativada, fazendo você se sentir lerdo!";
								textLog = "a slowness trap";
					
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
								m.FixedParticles( 0x3779, 10, 15, 5002, EffectLayer.Head );
								m.PlaySound( 0x1DF );
								m.RawDex = m.RawDex - 1; 
								sTrapType = textLog;
							}
						}
						else
						{
							if ( m.RawInt > 10 )
							{
								textSay = "Uma armadilha foi ativada, deixando sua mente nublada!";
								textLog = "a mind numbing trap";
					
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
								m.FixedParticles( 0x3779, 10, 15, 5004, EffectLayer.Head );
								m.PlaySound( 0x1E4 );
								m.RawInt = m.RawInt - 1;
								sTrapType = textLog;
							}
						}
					}
					else if ( nTrapType == 6 && SavingThrow( m, "Poison", true, this ) == false ) // POISON TRAP
					{
						textSay = "Uma armadilha foi ativada, fazendo você se sentir doente!";
						textLog = "a poison gas trap";
			
						int itHurts = m.PoisonResistance;
						int itSicks = 0;

						if ( itHurts >= 70 ){ itSicks = 1; }
						else if ( itHurts >= 50 ){ itSicks = 2; }
						else if ( itHurts >= 30 ){ itSicks = 3; }
						else if ( itHurts >= 10 ){ itSicks = 4; }
						else { itSicks = 5; }

						switch( Utility.RandomMinMax( 1, itSicks ) )
						{
							case 1: m.ApplyPoison( m, Poison.Lesser );	break;
							case 2: m.ApplyPoison( m, Poison.Regular );	break;
							case 3: m.ApplyPoison( m, Poison.Greater );	break;
							case 4: m.ApplyPoison( m, Poison.Deadly );	break;
							case 5: m.ApplyPoison( m, Poison.Lethal );	break;
						}

						Effects.SendLocationEffect( this.Location, this.Map, 0x11A8 - 2, 16, 3, 0, 0 );
						Effects.PlaySound( this.Location, this.Map, 0x231 );
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);

						sTrapType = textLog;
					}
					else if ( nTrapType == 7 && SavingThrow( m, "Magic", true, this ) == false ) // DRAIN TRAP
					{
						int nStat = Utility.RandomMinMax( 1, 3 );

						if ( nStat == 1 )
						{
							textSay = "Uma armadilha foi ativada, fazendo você se sentir quase morto!";
							textLog = "a life draining trap";
				
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							m.FixedParticles( 0x3779, 10, 15, 5009, EffectLayer.Waist );
							m.PlaySound( 0x1E6 );
							m.Hits = 1; 
							sTrapType = textLog;
						}
						else if ( nStat == 2 )
						{
							textSay = "Uma armadilha foi ativada, fazendo você se sentir muito cansado!";
							textLog = "a stamina draining trap";
				
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							m.FixedParticles( 0x3779, 10, 15, 5002, EffectLayer.Head );
							m.PlaySound( 0x1DF );
							m.Stam = 0; 
							sTrapType = textLog;
						}
						else
						{
							textSay = "Uma armadilha foi ativada, drenando sua mana!";
							textLog = "a mana draining trap";
				
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							m.FixedParticles( 0x3779, 10, 15, 5004, EffectLayer.Head );
							m.PlaySound( 0x1E4 );
							m.Mana = 0; 
							sTrapType = textLog;
						}
					}
					else if ( nTrapType == 8 && SavingThrow( m, "Magic", true, this ) == false ) // GEM STONE TRAP
					{
						List<Item> items = new List<Item>();
						int nAmount = 0;

						foreach( Item i in m.Backpack.FindItemsByType( typeof( Ruby ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( Amber ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( Amethyst ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( Citrine ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( Emerald ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( Diamond ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( Sapphire ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( StarSapphire ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( Tourmaline ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( DDRelicGem ), true ) ){ items.Add(i); }
						foreach( Item i in m.Backpack.FindItemsByType( typeof( MageEye ), true ) ){ items.Add(i); }

						foreach ( Item item in items )
						{
							if ( item != null )
							{
								nAmount = nAmount + item.Amount;
								item.Delete();
							}
						}
						if ( nAmount > 0 )
						{
							textSay = "Uma armadilha foi ativada, fazendo seus gemas se fundirem!";
							textLog = "a lode stone trap";
				
							RuinedGems rocks = new RuinedGems();
							rocks.Weight = nAmount * 5.0;
							rocks.RuinedCount = nAmount;
							m.AddToBackpack ( rocks );
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							m.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
							m.PlaySound( 0x1E1 );
							sTrapType = textLog;
						}
					}
					else if ( nTrapType == 9 && SavingThrow( m, "Magic", true, this ) == false ) // REAGENT TRAP
					{
						int nAmount = 0;

						if ( m != null && m.Backpack != null )
						{
							List<Item> list = new List<Item>();
							(m.Backpack).RecurseItems( list );
							foreach ( Item i in list )
							{
								if ( i.Catalog == Catalogs.Reagent )
								{
									nAmount = nAmount + i.Amount;
									i.Delete();
								}
							}
						}

						if ( nAmount > 0 )
						{
							textSay = "Você entrou em uma nuvem tóxica, estragando seus reagentes!";
							textLog = "a toxic cloud trap";

							RottedReagents regs = new RottedReagents();
							regs.Weight = nAmount * 0.1;
							regs.RottedCount = nAmount;
							m.AddToBackpack ( regs );
							Effects.SendLocationEffect( this.Location, this.Map, 0x11A8 - 2, 16, 3, 0, 0 );
							Effects.PlaySound( this.Location, this.Map, 0x231 );
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							sTrapType = textLog;
						}
					}
					else if ( nTrapType == 10 && SavingThrow( m, "Magic", true, this ) == false ) // BOOK BOUND TRAP
					{
						Container cont = m.Backpack;
						int nDull = 0;

						List<Item> items = new List<Item>();

						Item handy = m.FindItemOnLayer( Layer.OneHanded );
						if ( handy is Spellbook )
						{
							items.Add(handy); nDull = 1;
						}

						Item tally = m.FindItemOnLayer( Layer.Trinket );
						if ( tally is Spellbook )
						{
							items.Add(tally); nDull = 1;
						}

						foreach( Item i in m.Backpack.FindItemsByType( typeof( Spellbook ), true ) )
						{
							if (i.Parent is BookBox){} else
							{
								if ( i.LootType != LootType.Blessed )
								{
									if ( CheckInsuranceOnTrap( i, m ) )
									{
										m.LocalOverheadMessage(MessageType.Emote, 1150, true, "Um dos seus livros quase foi magicamente encadernado!");
									}
									else
									{
										items.Add(i); nDull = 1;
									}
								}
							}
						}
						foreach( Item i in m.Backpack.FindItemsByType( typeof( Runebook ), true ) )
						{
							if (i.Parent is BookBox){} else
							{
								items.Add(i);
								nDull = 1;
							}
						}

						if ( nDull > 0 )
						{
							Container box = new BookBox();
							foreach ( Item item in items )
							{
								box.DropItem(item);
							}

							m.AddToBackpack ( box );

							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "Uma armadilha foi ativada, trancando seus livros em uma caixa mágica!");
							m.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
							m.PlaySound( 0x1E1 );
							sTrapType = "a book bound trap";
						}
					}
					else if ( nTrapType == 11 && SavingThrow( m, "Magic", true, this ) == false ) // TELEPORT TRAP
					{
						textSay = "Uma armadilha foi ativada, teleportando você para longe daqui!";
						textLog = "a teleportation trap";
			
						Point3D p = Worlds.GetRandomLocation( m.Land, "land" );
						Map map = Worlds.GetMyDefaultMap( m.Land );

						if ( p != Point3D.Zero )
						{
							Server.Mobiles.BaseCreature.TeleportPets( m, p, map );
							m.MoveToWorld( p, map );
							Effects.PlaySound( m.Location, m.Map, 0x1FC );
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							sTrapType = textLog;
						}
					}
					else if ( nTrapType == 12  && SavingThrow( m, "Magic", true, this ) == false && m.Fame > 0 ) // FAME TRAP
					{
						int FameLoss = (int)(m.Fame - ( m.Fame * 0.20 ));
						if ( FameLoss < 0 ){ FameLoss = 0; }
						if ( FameLoss > 0 )
						{
							m.Fame = FameLoss;
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "Uma armadilha foi ativada, fazendo com que alguns de seus feitos sejam esquecidos!");
							m.FixedParticles( 0x374A, 10, 15, 5032, EffectLayer.Head );
							m.PlaySound( 0x1F8 );
							sTrapType = "a forgotten fame trap";
						}
					}
					else if ( nTrapType == 13 && SavingThrow( m, "Magic", true, this ) == false ) // CURSE ITEM TRAP
					{
						Container cont = m.Backpack;
						Item iCursed = GetMyItem( m );

						if ( iCursed != null )
						{
							if ( Mobile.InsuranceEnabled && CheckInsuranceOnTrap( iCursed, m ) )
							{
								m.LocalOverheadMessage(MessageType.Emote, 1150, true, "Um dos seus itens protegidos quase foi amaldiçoado!");
							}
							else
							{
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "Uma armadilha foi ativada, colocando uma maldição em um de seus itens equipados!");
								m.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
								m.PlaySound( 0x1E1 );

								Container box = new CurseItem();
								box.DropItem(iCursed);
								box.ItemID = iCursed.GraphicID;
								box.Hue = iCursed.GraphicHue;
								box.Name = iCursed.Name;

								m.AddToBackpack ( box );

								sTrapType = "a curse item trap";
							}
						}
					}
					else if ( nTrapType == 14 && SavingThrow( m, "Physical", true, this ) == false ) // FLOOR SPIKE TRAP
					{
						if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ Effects.SendLocationEffect( this.Location, this.Map, 4506 + 1, 18, 3, 0, 0 ); }
						else { Effects.SendLocationEffect( this.Location, this.Map, 4512 + 1, 18, 3, 0, 0 ); }
						Effects.PlaySound( this.Location, this.Map, 0x22C );
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "Você ativou uma armadilha de espinhos!");
						int itHurts = (int)( (Utility.RandomMinMax(50,200) * ( 100 - m.PhysicalResistance ) ) / 100 );
						m.Damage( itHurts, m );
						sTrapType = "a spike trap";
					}
					else if ( nTrapType == 15 && SavingThrow( m, "Physical", true, this ) == false ) // SAW TRAP
					{
						if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ Effects.SendLocationEffect( this.Location, this.Map, 0x11AC + 1, 6, 3, 0, 0 ); }
						else { Effects.SendLocationEffect( this.Location, this.Map, 0x11B1 + 1, 6, 3, 0, 0 ); }
						Effects.PlaySound( this.Location, this.Map, 0x21C );
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "Você ativou uma armadilha de lâmina serrilhada!");
						int itHurts = (int)( (Utility.RandomMinMax(50,200) * ( 100 - m.PhysicalResistance ) ) / 100 );
						m.Damage( itHurts, m );
						sTrapType = "a saw blade trap";
					}
					else if ( nTrapType == 16 && SavingThrow( m, "Fire", true, this ) == false ) // FLAME TRAP
					{
						textSay = "Você ativou uma armadilha de fogo!";
						textLog = "a fire trap";
			
						Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3709, 10, 30, 5052 );
						Effects.PlaySound( this.Location, this.Map, 0x225 );
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
						int itHurts = (int)( (Utility.RandomMinMax(50,200) * ( 100 - m.FireResistance ) ) / 100 );
						m.Damage( itHurts, m );
						sTrapType = textLog;
					}
					else if ( nTrapType == 17 && SavingThrow( m, "Physical", true, this ) == false ) // GIANT SPIKE TRAP
					{
						Effects.SendLocationEffect( this.Location, this.Map, 0x1D99, 48, 2, 0, 0 );
						Effects.PlaySound( this.Location, this.Map, 0x22C );
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "Você ativou uma armadilha de espinhos gigantes!");
						int itHurts = (int)( (Utility.RandomMinMax(50,200) * ( 100 - m.PhysicalResistance ) ) / 100 );
						m.Damage( itHurts, m );
						sTrapType = "a giant spike trap";
					}
					else if ( nTrapType == 18 && SavingThrow( m, "Fire", true, this ) == false ) // EXPLOSION TRAP
					{
						textSay = "Você ativou uma armadilha de explosão!";
						textLog = "an explosion trap";
			
						m.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
						m.PlaySound( 0x307 );
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
						int itHurts = (int)( (Utility.RandomMinMax(50,200) * ( 100 - m.FireResistance ) ) / 100 );
						m.Damage( itHurts, m );
						sTrapType = textLog;
					}
					else if ( nTrapType == 19 && SavingThrow( m, "Energy", true, this ) == false ) // ELECTRICAL TRAP
					{
						textSay = "Você ativou uma armadilha elétrica!";
						textLog = "an electrical trap";
			
						m.BoltEffect( 0 );
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
						int itHurts = (int)( (Utility.RandomMinMax(50,200) * ( 100 - m.EnergyResistance ) ) / 100 );
						m.Damage( itHurts, m );
						sTrapType = textLog;
					}
					else if ( nTrapType == 20 && SavingThrow( m, "Agility", true, this ) == false ) // TRIP WIRE THAT BREAKS ARROWS
					{
						List<Item> items = new List<Item>();
						int nBroken = 0;
						int WhichArrows = Utility.RandomMinMax( 1, 2 );
						string sTripped = "";
						int nAmount = 0;

						if ( WhichArrows == 1 )
						{
							foreach( Item i in m.Backpack.FindItemsByType( typeof( Arrow ), true ) )
							{
								items.Add(i);
								nBroken = 1;
								sTripped = "arrows";
							}
						}
						else
						{
							foreach( Item i in m.Backpack.FindItemsByType( typeof( Bolt ), true ) )
							{
								items.Add(i);
								nBroken = 1;
								sTripped = "crossbow bolts";
							}
						}
						if ( nBroken > 0 )
						{
							foreach ( Item item in items )
							{
								if ( item != null )
								{
									nAmount = nAmount + item.Amount;
									item.Delete();
								}
							}
							if ( nAmount > 0 )
							{
								textSay = "Você tropeçou em um fio e quebrou todos os seus " + sTripped + "!";
								textLog = "a trip wire trap";
					
								Shaft wood = new Shaft();
								wood.Amount = nAmount;
								m.AddToBackpack ( wood );

								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
								m.PlaySound( m.Female ? 812 : 1086 );
								sTrapType = textLog;
							}
						}
					}
					else if ( nTrapType == 21 && SavingThrow( m, "Poison", true, this ) == false ) // TAINTED TRAP
					{
						List<Item> items = new List<Item>();
						int nAmount = 0;

						foreach( Item i in m.Backpack.FindItemsByType( typeof( Bandage ), true ) )
						{
							items.Add(i);
						}
						foreach ( Item item in items )
						{
							if ( item != null )
							{
								nAmount = nAmount + item.Amount;
								item.Delete();
							}
						}
						if ( nAmount > 0 )
						{
							TaintedBandage bandage = new TaintedBandage();
							bandage.Weight = nAmount * 0.1;
							string sAmount = nAmount.ToString();
							if ( nAmount > 1 ){ bandage.Name = sAmount + " tainted bandages"; }
							m.AddToBackpack ( bandage );

							Effects.SendLocationEffect( this.Location, this.Map, 0x11A8 - 2, 16, 3, 0, 0 );
							Effects.PlaySound( this.Location, this.Map, 0x231 );
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "Você entrou em uma nuvem nociva, contaminando suas bandagens!");

							sTrapType = "a noxious cloud trap";
						}
					}
					else if ( nTrapType == 22 && SavingThrow( m, "Agility", true, this ) == false ) // TRIP WIRE THAT BREAKS POTIONS
					{
						List<Item> items = new List<Item>();
						int nBroken = 0;

						if ( m != null && m.Backpack != null )
						{
							List<Item> list = new List<Item>();
							(m.Backpack).RecurseItems( list );
							foreach ( Item i in list )
							{
								if ( i.Catalog == Catalogs.Potion )
								{
									nBroken = 1;
									i.Delete();
								}
							}
						}

						if ( nBroken > 0 )
						{
							textSay = "Você tropeçou em um fio e quebrou todos os seus frascos de poção!";
							textLog = "a trip wire trap";
				
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							m.PlaySound( 0x040 );
							sTrapType = textLog;
						}
					}
					else if ( nTrapType == 23 && SavingThrow( m, "Magic", true, this ) == false ) // MELT JEWELS TRAP
					{
						int puddle = 0;

						if ( m != null && m.Backpack != null )
						{
							List<Item> list = new List<Item>();
							Item jw;

							if ( m.FindItemOnLayer( Layer.Bracelet ) != null ) { jw = m.FindItemOnLayer( Layer.Bracelet ); if ( jw.LootType != LootType.Blessed && jw is BaseTrinket && jw.Catalog == Catalogs.Jewelry ){ list.Add(jw); } }
							if ( m.FindItemOnLayer( Layer.Ring ) != null ) { jw = m.FindItemOnLayer( Layer.Ring ); if ( jw.LootType != LootType.Blessed && jw is BaseTrinket && jw.Catalog == Catalogs.Jewelry ){ list.Add(jw); } }
							if ( m.FindItemOnLayer( Layer.Helm ) != null ) { jw = m.FindItemOnLayer( Layer.Helm ); if ( jw.LootType != LootType.Blessed && jw is BaseTrinket && jw.Catalog == Catalogs.Jewelry ){ list.Add(jw); } }
							if ( m.FindItemOnLayer( Layer.Neck ) != null ) { jw = m.FindItemOnLayer( Layer.Neck ); if ( jw.LootType != LootType.Blessed && jw is BaseTrinket && jw.Catalog == Catalogs.Jewelry ){ list.Add(jw); } }
							if ( m.FindItemOnLayer( Layer.Earrings ) != null ) { jw = m.FindItemOnLayer( Layer.Earrings ); if ( jw.LootType != LootType.Blessed && jw is BaseTrinket && jw.Catalog == Catalogs.Jewelry ){ list.Add(jw); } }

							(m.Backpack).RecurseItems( list );
							foreach ( Item i in list )
							{
								if ( i is BaseTrinket && i.Catalog == Catalogs.Jewelry )
								{
									i.Delete();
									puddle++;
								}
							}
						}

						if ( puddle > 0 )
						{
							textSay = "Uma armadilha foi ativada, derretendo todas as suas joias!";
							textLog = "a jewelry melting trap";
				
							m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
							RustyJunk broke = new RustyJunk();
							broke.ItemID = 0x122A;
							broke.Name = "melted jewelry";
							broke.Hue = 0x9C4;
							broke.Weight = puddle;
							m.AddToBackpack ( broke );
							sTrapType = textLog;
						}
					}
					else if ( nTrapType == 24 && SavingThrow( m, "Agility", true, this ) == false ) // PIT TRAP
					{
						textSay = "Uma queda em um poço profundo!";
						textLog = "a deep pit";

						string sX = m.X.ToString();
						string sY = m.Y.ToString();
						string sZ = m.Z.ToString();
						string sMap = Worlds.GetMyMapString( m.Map );
						string sZone = Server.Lands.LandName( Server.Lands.GetLand( m.Map, m.Location, m.X, m.Y ) );

						((PlayerMobile)m).CharacterPublicDoor = sX + "#" + sY + "#" + sZ + "#" + sMap + "#" + sZone;

						Effects.PlaySound( m.Location, m.Map, Utility.RandomList( 0x5D2,0x5D3 ) );
						Point3D p = new Point3D( 2602, 3688, 100 );
						Point3D b = new Point3D( 2602, 3688, 0 );
						Map map = Map.Sosaria;

						Server.Mobiles.BaseCreature.TeleportPets( m, b, map );
						m.MoveToWorld( p, map );
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, textSay);
						sTrapType = textLog;
					}
					else if ( nTrapType == 25 && m.Karma != 0 && SavingThrow( m, "Magic", true, this ) == false ) // ALIGNMENT TRAP
					{
						m.Karma = 0;
						m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "Uma armadilha foi ativada, distorcendo sua moralidade de volta a um estado neutro!");
						m.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
						m.PlaySound( 0x1E1  );
						sTrapType = "a mind warping trap";
					}

					///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

					if ( sTrapType != "" )
					{
						if ( m.Hidden != false )
						{
							m.RevealingAction();
						}

						HadAnyAffect = true; 

						LoggingFunctions.LogTraps( m, sTrapType );
					}
				}

				if ( HadAnyAffect || HiddenTrapType == 1000 )
					DisableTrap( this );
				else
				{
					if ( Weight == 3.0 )
					{
						DisableTrap( this );
						this.Name = "a broken trap";
						m.PlaySound( 0x41 ); // glass breaking
						m.SendMessage( "Você pisou em uma armadilha mas sorte sua ela se quebrou!");
					}	
					else
						DitchTrap( this );

				}
			}

			if ( sTrapType == "a teleportation trap" || sTrapType == "an overcharged transporter pad" || sTrapType == "a deep pit" )
				return false;

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( HiddenTrapType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            HiddenTrapType = reader.ReadInt();
			if ( Weight != 1.0 )
				Delete();
		}

		public static bool IAmAWeaponSlayer( Mobile m, Mobile enemy )
		{
			bool IsSlayer = false;

			if ( m is PlayerMobile )
			{
				if ( m.FindItemOnLayer( Layer.OneHanded ) is BaseWeapon )
				{
					Item hitter = m.FindItemOnLayer( Layer.OneHanded );
					BaseWeapon weapon = (BaseWeapon)hitter;
					SlayerName slay1 = weapon.Slayer;
					SlayerName slay2 = weapon.Slayer2;
					if ( slay1 != SlayerName.None )
					{
						SlayerEntry entry1 = SlayerGroup.GetEntryByName( slay1 );
						if ( entry1.Slays( enemy ) ){ IsSlayer = true; }
					}
					if ( slay2 != SlayerName.None )
					{
						SlayerEntry entry2 = SlayerGroup.GetEntryByName( slay2 );
						if ( entry2.Slays( enemy ) ){ IsSlayer = true; }
					}
				}
				else if ( m.FindItemOnLayer( Layer.TwoHanded ) is BaseWeapon )
				{
					Item hitter = m.FindItemOnLayer( Layer.TwoHanded );
					BaseWeapon weapon = (BaseWeapon)hitter;
					SlayerName slay1 = weapon.Slayer;
					SlayerName slay2 = weapon.Slayer2;
					if ( slay1 != SlayerName.None )
					{
						SlayerEntry entry1 = SlayerGroup.GetEntryByName( slay1 );
						if ( entry1.Slays( enemy ) ){ IsSlayer = true; }
					}
					if ( slay2 != SlayerName.None )
					{
						SlayerEntry entry2 = SlayerGroup.GetEntryByName( slay2 );
						if ( entry2.Slays( enemy ) ){ IsSlayer = true; }
					}
				}
			}
			return IsSlayer;
		}

		public static bool IAmShielding( Mobile m, int skill )
		{
			bool Shielded = false;

			if ( m is PlayerMobile )
			{
				if ( m.FindItemOnLayer( Layer.TwoHanded ) is BaseShield )
				{
					if ( m.CheckSkill( SkillName.Parry, 0, skill ) )
					{
						Shielded = true;
					}
				}
			}
			return Shielded;
		}

		public static bool SavingThrow( Mobile m, string save, bool isTrap, Item trap )
		{
			bool madeSave = false;
			int SaveThrow = 0;
			string area = "";

			if ( save == "Magic" )
			{
				area = "magical resistance";
				SaveThrow = (int)(( m.Int + m.Skills[SkillName.MagicResist].Value + m.EnergyResistance ) / 4);
			}
			else if ( save == "Physical" )
			{
				area = "physical resistance";
				SaveThrow = (int)(( m.Str + m.PhysicalResistance ) / 3);
			}
			else if ( save == "Agility" )
			{
				area = "quick reflexes";
				SaveThrow = m.Dex;
			}
			else if ( save == "Cold" )
			{
				area = "cold resistance";
				SaveThrow = (int)(( m.Dex + m.ColdResistance ) / 3);
			}
			else if ( save == "Fire" )
			{
				area = "fire resistance";
				SaveThrow = (int)(( m.Dex + m.FireResistance ) / 3);
			}
			else if ( save == "Poison" )
			{
				area = "poison resistance";
				SaveThrow = (int)(( m.Str + m.Skills[SkillName.Poisoning].Value + m.PoisonResistance ) / 4);
			}
			else if ( save == "Energy" )
			{
				area = "energy resistance";
				SaveThrow = (int)(( m.Int + m.EnergyResistance ) / 3);
			}

			if ( SaveThrow > 60 ){ SaveThrow = 60; }

			if ( SaveThrow >= Utility.RandomMinMax( 1, 100 ) )
			{
				if ( isTrap )
				{
					string textSay = "Você chegou perto de uma armadilha escondida, mas com seu " + area + "... você a evita.";
					m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, textSay);
					m.PlaySound( m.Female ? 778 : 1049 );

					if ( trap is HiddenTrap )
						((HiddenTrap)trap).HiddenTrapType = 1000;
				}
				madeSave = true;
			}

			return madeSave;
		}

		public static bool CheckInsuranceOnTrap( Item item, Mobile m )
		{
			if ( item.LootType == LootType.Blessed )
			{
				return true;
			}
			else if ( Mobile.InsuranceEnabled && item.Insured )
			{
				PlayerMobile pm = (PlayerMobile)m;

				if ( pm.AutoRenewInsurance )
				{
					int cost = 900;

					if ( Banker.Withdraw( m, cost ) )
					{
						item.PayedInsurance = true;
						m.SendLocalizedMessage(1060398, cost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
					}
					else
					{
						m.SendLocalizedMessage( 1061079, "", 0x23 ); // You lack the funds to purchase the insurance
						item.PayedInsurance = false;
						item.Insured = false;
					}
				}
				else
				{
					item.PayedInsurance = false;
					item.Insured = false;
				}
				return true;
			}

			return false;
		}

		public static bool CanSetOffTraps( Mobile m )
		{
			if ( m is PlayerMobile && ( !m.Alive || m.Blessed || m.AccessLevel > AccessLevel.Player ) )
				return false;

			return true;
		}

		public static bool PassiveSearching( Item item, Mobile m )
		{
			if ( m.Skills.Searching.Value >= 5 && m is PlayerMobile )
			{
				if ( item.Weight <= 2.0 && HiddenTrap.SeeIfTrapActive( item ) && m.CheckSkill( SkillName.Searching, 0, 125 ) )
				{
					string textSay = "Há uma armadilha oculta no chão sob seus pés!";
					
					m.PlaySound( m.Female ? 778 : 1049 ); m.Say( "*ah!*" );
					m.SendMessage( textSay );
					item.Visible = true;
					item.Weight = 3.0;
					SetAppearance( item );
					new Delete_5_Minutes( item ).Start();

					return true;
				}
			}
			return false;
		}

		public static bool SeeIfTrapActive( Item trap )
		{
			if ( trap.Weight < 2.0 && trap is HiddenTrap && Utility.RandomMinMax( 1, 100 ) > MyServerSettings.FloorTrapTrigger() )
			{
				DitchTrap( trap );
				return false;
			}
			else if ( trap.Weight < 2.0 )
				trap.Weight = 2.0;

			return true;
		}

		public static void DitchTrap( Item trap )
		{
			if ( trap.Weight < 5.0 && trap is HiddenTrap )
			{
				trap.Visible = false;
				trap.Weight = 6.0;
				SetAppearance( trap );
				new Delete_5_Seconds( trap ).Start();
			}
		}

		public static void DisableTrap( Item trap )
		{
			if ( trap.Weight < 5.0 && trap is HiddenTrap )
			{
				trap.Visible = true;
				trap.Weight = 5.0;
				SetAppearance( trap );
				new Delete_5_Minutes( trap ).Start();
			}
		}

		public static void DiscoverTrap( Item trap )
		{
			if ( trap.Weight < 3.0 && trap is HiddenTrap )
			{
				trap.Visible = true;
				trap.Weight = 3.0;
				SetAppearance( trap );
				new Delete_5_Minutes( trap ).Start();
			}
		}

		public static bool CheckTrapAvoidance( Mobile m, Item Trap )
		{
			string textSay = "";

			bool nSprung = true;

			if ( m.Skills.RemoveTrap.Value >= 5 )
			{
				if ( m is PlayerMobile && m.CheckSkill( SkillName.RemoveTrap, 0, 125 ) )
				{
					if ( Trap is MushroomTrap )
					{
						m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, "Você chegou perto de um cogumelo estranho, mas sua habilidade em remover armadilhas o ajudou a evitá-lo.");
					}
					else
					{
						textSay = "Você chegou perto de uma armadilha escondida, mas sua habilidade em remover armadilhas o ajudou a desativá-la.";
						m.PlaySound( m.Female ? 0x32E : 0x440 );
						m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, textSay);
						m.PlaySound( 0x241 );
					}
				}
				nSprung = false;
			}

			// Berserker Uncanny Dodge
			if (m is PlayerMobile && nSprung)
			{
			    PlayerMobile pm = m as PlayerMobile;

			    if (pm.ActiveAscension == AscensionType.Berserker && pm.AscensionProfile != null)
			    {
			        AscensionProgress prog = pm.AscensionProfile.Get(AscensionType.Berserker);

			        if (prog != null && prog.Level >= 8)
			        {
			            int level = prog.Level;

			            double chance;

			            if (level >= 17)
			                chance = 0.45 + (0.01 * level);
			            else
			                chance = 0.25 + (0.01 * level);

			            if (Utility.RandomDouble() <= chance)
			            {
			                m.LocalOverheadMessage(
			                    Network.MessageType.Emote,
			                    0x3B2,
			                    false,
			                    "You instinctively sense danger and avoid a trap!"
			                );
			                m.PlaySound(0x1F2);
			                nSprung = false;
			            }
			        }
			    }
			}

			if ( m is PlayerMobile )
			{
				if ( m.Backpack != null && nSprung )
				{
					Item magicwand = m.Backpack.FindItemByType( typeof ( TrapWand ) );

					if ( magicwand != null )
					{
						TrapWand wands = (TrapWand)magicwand;
						int nPower = wands.WandPower;
						if ( nPower >= Utility.RandomMinMax( 1, 100 ) && wands.owner == m )
						{
							if ( Trap is MushroomTrap )
							{
								m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, "Você chegou perto de um cogumelo estranho, mas a magia da sua orbe o ajudou a evitá-lo.");
							}
							else
							{
								textSay = "Você chegou perto de uma armadilha escondida, mas a magia da sua orbe a desativou.";
								m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, textSay);
							}
							m.PlaySound( 0x1F0 );
							nSprung = false;
						}
					}
				}
				if ( GetPlayerInfo.LuckyPlayer(m.Luck) && nSprung )
				{
					if ( Trap is MushroomTrap )
					{
						m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, "Você chegou perto de um cogumelo estranho, mas com a sorte ao seu lado... você o evita.");
					}
					else
					{
						textSay = "Você chegou perto de uma armadilha escondida, mas com a sorte ao seu lado... ela se quebrou.";
						m.PlaySound( 0x241 ); 
						m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, textSay);
					}
					nSprung = false;
				}
				if ( m.Backpack != null && nSprung )
				{
					Item tenfootpole = m.Backpack.FindItemByType( typeof ( TenFootPole ) );

					if ( tenfootpole != null )
					{
						TenFootPole poles = (TenFootPole)tenfootpole;
						if ( poles.Tap >= Utility.RandomMinMax( 1, 100 ) )
						{
							m.PlaySound( 0x3FD );
							poles.ConsumeLimits( 1 );
							if ( poles.Limits < 1 )
							{
								if ( Trap is MushroomTrap )
								{
									m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, "Você quebra sua vara de dez pés, mas evita o cogumelo estranho por perto.");
								}
								else
								{
									textSay = "Você toca sua vara de dez pés, desativando uma armadilha escondida e quebrando a vara.";
									m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, textSay);
								}
							}
							else
							{
								if ( Trap is MushroomTrap )
								{
									m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, "Você toca sua vara de dez pés, evitando um cogumelo estranho por perto.");
								}
								else
								{
									textSay = "Você toca sua vara de dez pés, desativando uma armadilha escondida.";
									m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, textSay);
								}
								poles.InvalidateProperties();
							}
							nSprung = false;
						}
					}
				}
			}

			return nSprung;
		}

		public static Item GetMyItem( Mobile m )
		{
			if ( m == null )
				return null;

			Item myItem = null;
			Item myBlessCheck = null;
			int cycle = 0;

			int nOuterTorso = 0;
			int nOneHanded = 0;
			int nTwoHanded = 0;
			int nBracelet = 0;
			int nRing = 0;
			int nHelm = 0;
			int nArms = 0;
			int nOuterLegs = 0;
			int nNeck = 0;
			int nGloves = 0;
			int nTalisman = 0;
			int nShoes = 0;
			int nCloak = 0;
			int nFirstValid = 0;
			int nWaist = 0;
			int nInnerLegs = 0;
			int nInnerTorso = 0;
			int nPants = 0;
			int nShirt = 0;
			int nEarrings = 0;

			if ( m.FindItemOnLayer( Layer.OuterTorso ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.OuterTorso ); if ( myBlessCheck.LootType == LootType.Blessed ){ nOuterTorso = 1; } }
			if ( m.FindItemOnLayer( Layer.OneHanded ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.OneHanded ); if ( myBlessCheck.LootType == LootType.Blessed ){ nOneHanded = 1; } }
			if ( m.FindItemOnLayer( Layer.TwoHanded ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.TwoHanded ); if ( myBlessCheck.LootType == LootType.Blessed ){ nTwoHanded = 1; } }
			if ( m.FindItemOnLayer( Layer.Bracelet ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Bracelet ); if ( myBlessCheck.LootType == LootType.Blessed ){ nBracelet = 1; } }
			if ( m.FindItemOnLayer( Layer.Ring ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Ring ); if ( myBlessCheck.LootType == LootType.Blessed ){ nRing = 1; } }
			if ( m.FindItemOnLayer( Layer.Helm ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Helm ); if ( myBlessCheck.LootType == LootType.Blessed ){ nHelm = 1; } }
			if ( m.FindItemOnLayer( Layer.Arms ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Arms ); if ( myBlessCheck.LootType == LootType.Blessed ){ nArms = 1; } }
			if ( m.FindItemOnLayer( Layer.OuterLegs ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.OuterLegs ); if ( myBlessCheck.LootType == LootType.Blessed ){ nOuterLegs = 1; } }
			if ( m.FindItemOnLayer( Layer.Neck ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Neck ); if ( myBlessCheck.LootType == LootType.Blessed ){ nNeck = 1; } }
			if ( m.FindItemOnLayer( Layer.Gloves ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Gloves ); if ( myBlessCheck.LootType == LootType.Blessed ){ nGloves = 1; } }
			if ( m.FindItemOnLayer( Layer.Trinket ) != null ) { if (!( m.FindItemOnLayer( Layer.Trinket ) is Spellbook )){ myBlessCheck = m.FindItemOnLayer( Layer.Trinket ); if ( myBlessCheck.LootType == LootType.Blessed ){ nTalisman = 1; } } }
			if ( m.FindItemOnLayer( Layer.Shoes ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Shoes ); if ( myBlessCheck.LootType == LootType.Blessed ){ nShoes = 1; } }
			if ( m.FindItemOnLayer( Layer.Cloak ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Cloak ); if ( myBlessCheck.LootType == LootType.Blessed ){ nCloak = 1; } }
			if ( m.FindItemOnLayer( Layer.FirstValid ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.FirstValid ); if ( myBlessCheck.LootType == LootType.Blessed ){ nFirstValid = 1; } }
			if ( m.FindItemOnLayer( Layer.Waist ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Waist ); if ( myBlessCheck.LootType == LootType.Blessed ){ nWaist = 1; } }
			if ( m.FindItemOnLayer( Layer.InnerLegs ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.InnerLegs ); if ( myBlessCheck.LootType == LootType.Blessed ){ nInnerLegs = 1; } }
			if ( m.FindItemOnLayer( Layer.InnerTorso ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.InnerTorso ); if ( myBlessCheck.LootType == LootType.Blessed ){ nInnerTorso = 1; } }
			if ( m.FindItemOnLayer( Layer.Pants ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Pants ); if ( myBlessCheck.LootType == LootType.Blessed ){ nPants = 1; } }
			if ( m.FindItemOnLayer( Layer.Shirt ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Shirt ); if ( myBlessCheck.LootType == LootType.Blessed ){ nShirt = 1; } }
			if ( m.FindItemOnLayer( Layer.Earrings ) != null ) { myBlessCheck = m.FindItemOnLayer( Layer.Earrings ); if ( myBlessCheck.LootType == LootType.Blessed ){ nEarrings = 1; } }

			while ( cycle > 20 )
			{
				cycle++;

				switch( Utility.RandomMinMax( 1, 20 ) )
				{
					case 1: if ( m.FindItemOnLayer( Layer.Waist ) != null && nWaist != 1) { myItem = m.FindItemOnLayer( Layer.Waist ); } break;
					case 2: if ( m.FindItemOnLayer( Layer.OuterTorso ) != null && nOuterTorso != 1) { myItem = m.FindItemOnLayer( Layer.OuterTorso ); } break;
					case 3: if ( m.FindItemOnLayer( Layer.OneHanded ) != null && nOneHanded != 1) { myItem = m.FindItemOnLayer( Layer.OneHanded ); } break;
					case 4: if ( m.FindItemOnLayer( Layer.TwoHanded ) != null && nTwoHanded != 1) { myItem = m.FindItemOnLayer( Layer.TwoHanded ); } break;
					case 5: if ( m.FindItemOnLayer( Layer.Bracelet ) != null && nBracelet != 1) { myItem = m.FindItemOnLayer( Layer.Bracelet ); } break;
					case 6: if ( m.FindItemOnLayer( Layer.Ring ) != null && nRing != 1) { myItem = m.FindItemOnLayer( Layer.Ring ); } break;
					case 7: if ( m.FindItemOnLayer( Layer.Helm ) != null && nHelm != 1) { myItem = m.FindItemOnLayer( Layer.Helm ); } break;
					case 8: if ( m.FindItemOnLayer( Layer.Arms ) != null && nArms != 1) { myItem = m.FindItemOnLayer( Layer.Arms ); } break;
					case 9: if ( m.FindItemOnLayer( Layer.OuterLegs ) != null && nOuterLegs != 1) { myItem = m.FindItemOnLayer( Layer.OuterLegs ); } break;
					case 10: if ( m.FindItemOnLayer( Layer.Neck ) != null && nNeck != 1) { myItem = m.FindItemOnLayer( Layer.Neck ); } break;
					case 11: if ( m.FindItemOnLayer( Layer.Gloves ) != null && nGloves != 1) { myItem = m.FindItemOnLayer( Layer.Gloves ); } break;
					case 12: if ( m.FindItemOnLayer( Layer.Trinket ) != null && nTalisman != 1) { myItem = m.FindItemOnLayer( Layer.Trinket ); } break;
					case 13: if ( m.FindItemOnLayer( Layer.Shoes ) != null && nShoes != 1) { myItem = m.FindItemOnLayer( Layer.Shoes ); } break;
					case 14: if ( m.FindItemOnLayer( Layer.Cloak ) != null && nCloak != 1) { myItem = m.FindItemOnLayer( Layer.Cloak ); } break;
					case 15: if ( m.FindItemOnLayer( Layer.FirstValid ) != null && nFirstValid != 1) { myItem = m.FindItemOnLayer( Layer.FirstValid ); } break;
					case 16: if ( m.FindItemOnLayer( Layer.InnerLegs ) != null && nInnerLegs != 1) { myItem = m.FindItemOnLayer( Layer.InnerLegs ); } break;
					case 17: if ( m.FindItemOnLayer( Layer.InnerTorso ) != null && nInnerTorso != 1) { myItem = m.FindItemOnLayer( Layer.InnerTorso ); } break;
					case 18: if ( m.FindItemOnLayer( Layer.Pants ) != null && nPants != 1) { myItem = m.FindItemOnLayer( Layer.Pants ); } break;
					case 19: if ( m.FindItemOnLayer( Layer.Shirt ) != null && nShirt != 1) { myItem = m.FindItemOnLayer( Layer.Shirt ); } break;
					case 20: if ( m.FindItemOnLayer( Layer.Earrings ) != null && nEarrings != 1) { myItem = m.FindItemOnLayer( Layer.Earrings ); } break;
				}

				if ( myItem != null )
					cycle = 20;
			}

			if ( myItem != null && myItem.Density != Density.None && ((int)(myItem.Density)*7) > Utility.Random( 100 ) )
				myItem = null;

			return myItem;
		}

		public override bool HandlesOnMovement{ get{ return true; } }

		private DateTime m_NextSound;	
		public DateTime NextSound{ get{ return m_NextSound; } set{ m_NextSound = value; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if( m is PlayerMobile )
			{
				if ( DateTime.Now >= m_NextSound && Utility.InRange( m.Location, this.Location, 10 ) )
				{
					if ( Utility.RandomBool() )
					{
						int sound = HiddenChest.DungeonSounds( this );	
						m.PlaySound( sound );	
					}
					m_NextSound = (DateTime.Now + TimeSpan.FromSeconds( 60 ));	
				}
			}
		}

		private class Delete_5_Seconds : Timer
		{
			private Item m_Trap;

			public Delete_5_Seconds( Item trap ) : base( TimeSpan.FromSeconds( 5.0 ) )
			{
				Priority = TimerPriority.OneSecond;
				m_Trap = trap;
			}

			protected override void OnTick()
			{
				m_Trap.Delete();
			}
		}

		private class Delete_5_Minutes : Timer
		{
			private Item m_Trap;

			public Delete_5_Minutes( Item trap ) : base( TimeSpan.FromMinutes( 5.0 ) )
			{
				Priority = TimerPriority.OneMinute;
				m_Trap = trap;
			}

			protected override void OnTick()
			{
				m_Trap.Delete();
			}
		}
	}
}