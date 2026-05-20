using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Misc;

namespace Server.Scripts.Custom
{
    public enum CitizenClass
    {
        Wizard = 1,
        Fighter = 2,
        Rogue = 3
    }

    [CorpseName("an adventurer corpse")]
    public class AdventurerTeam : BaseCreature
    {
        #region Dialogue Data (Static / Flavor Text)
        private static readonly string[] FriendlyChat = new string[]
        {
            "Os dragões nas cavernas profundas ficam mais ousados a cada dia...",
            "Quase escapei de uma matilha de lobos ferozes ontem.",
            "Estas terras são amaldiçoadas, lhe digo. O mal se agita nas sombras.",
            "Vamos explorar aquela masmorra, eles disseram. Vai ser divertido, eles disseram.",
            "Vi a sombra de um dragão passar no céu ontem à noite.",
            "Cuidado - armadilhas abundam nestas ruínas antigas.",
            "Os ursos-troll têm atacado caravanas novamente.",
            "Dizem que uma tumba esquecida jaz em algum lugar por aqui.",
            "Procuro por uma lâmina lendária, perdida no tempo.",
            "Tesouros antigos aguardam aqueles corajosos o suficiente para reivindicá-los.",
            "Um comerciante falou de ruínas cheias de ouro e joias nas proximidades.",
            "A torre do velho mago supostamente guarda grande poder.",
            "Ouvi sussurros sobre um cofre escondido nas montanhas.",
            "Meus companheiros caíram em uma emboscada há três dias.",
            "Procuro almas corajosas para mergulhar na escuridão.",
            "Viajar sozinho por estas partes é sentença de morte.",
            "Perdi todo o meu grupo para um demônio nas profundezas inferiores.",
            "Precisaríamos de outro braço armado para o que está por vir.",
            "Estou ficando sem suprimentos... preciso reabastecer logo.",
            "Há curandeiros por perto? Minhas feridas ainda doem.",
            "Pagaria bom dinheiro por poções de cura de qualidade.",
            "Estas ataduras velhas não vão durar muito mais.",
            "Preciso de armadura melhor antes de me aventurar mais fundo.",
            "Assassinos de capa vermelha foram avistados perto da encruzilhada!",
            "Disseram que orcs têm atacado fazendas no norte.",
            "Cuidado com os cavaleiros negros, pois eles não conhecem piedade.",
            "Um bando de salteadores acampa a não mais de uma milha ao norte daqui.",
            "Fique longe das florestas do sul após o anoitecer.",
            "As velhas lendas falam de poder selado nestas ruínas.",
            "Luzes estranhas dançam no cemitério à meia-noite.",
            "Vi coisas lá embaixo que desafiam qualquer explicação.",
            "Os antigos deixaram mais do que apenas seus ossos para trás.",
            "Rituais sombrios estão sendo realizados nos níveis inferiores.",
            "O ar fica mais frio quanto mais fundo se vai — isso nunca é um bom sinal.",
            "Marquei um caminho seguro através dos escombros, mas não durará para sempre.",
            "Algo lá embaixo caça pelo som... pise leve.",
            "Selamos uma passagem atrás de nós — o que quer que estivesse dentro não ficou feliz.",
            "Encontramos uma parede coberta de runas, ainda brilhando fracamente.",
            "Encontrei marcas de garras onde nenhuma fera deveria caber.",
            "Aqueles corredores se torcem sobre si mesmos!",
            "Os mortos não permanecem mortos aqui.",
            "Ouvi cantos ecoando pelos salões, mas não encontrei ninguém.",
            "Um túnel desabado quase nos enterrou vivos.",
            "Há sangue fresco na pedra... e não é meu.",
            "Perdemos nosso portador de tochas para a escuridão adiante.",
            "O que quer que guarde o santuário interior ainda está vivo.",
            "Juro que as paredes estavam nos observando.",
            "Algumas portas é melhor deixar fechadas.",
            "O chão cedeu sob nós — cuidado onde pisa.",
            "Nunca vi magia perdurar assim antes.",
            "O silêncio lá embaixo é o pior de tudo.",
            "Viramos para trás quando as tochas começaram a falhar.",
            "A luz é sua melhor aliada nessas profundezas."
        };

		// Evil adventurers - for murderers and dark warriors
		private static readonly string[] EvilChat = new string[]
        {
            "Sua moeda ou sua vida, tolo.",
            "Os abutres vão se banquetear esta noite...",
            "Este é NOSSO território. Pague o pedágio ou sangre.",
            "Os fracos existem apenas para servir aos fortes.",
            "Sinto cheiro de medo... e ouro.",
            "Cinco cadáveres antes do meio-dia. Boa caçada hoje.",
            "Seus gritos ainda ecoam em meus ouvidos.",
            "Deixei um rastro de corpos daqui até a costa.",
            "O rio corre vermelho com o sangue deles.",
            "Perdi a conta de quantos matei este mês.",
            "Estas ruínas pertencem a nós agora. Vá ou junte-se aos mortos.",
            "Volte enquanto ainda respira.",
            "Apenas os fortes sobrevivem aqui. Você não parece forte.",
            "Invasores acabam alimentando os corvos.",
            "Este lugar é nosso. Encontre sua própria cova para roubar.",
            "Precisa de alguém morto? Conheço pessoas...",
            "Pelo preço certo, qualquer um pode desaparecer.",
            "Não fazemos perguntas. Apenas coletamos cabeças.",
            "Ouro fala. Misericórdia não.",
            "Honra é para os mortos e os tolos.",
            "No final, só o poder importa.",
            "A escuridão acolhe todos que a abraçam.",
            "Suas leis não têm poder aqui!",
            "Moralidade é um luxo que não podemos pagar.",
            "Eles nunca encontrarão o corpo dela...",
            "Ei, um saco de moedas acabou de entrar...",
            "Gosto de ver a luz se apagar.",
            "Não se pode ser fraco por estas bandas. Mate-os.",
            "O sangue afia minha lâmina.",
            "Vá agora, ou será mais um cadáver no meu caminho!",
            "Esses covardes finalmente encontrarão seu fim!",
            "A masmorra nos alimenta bem.",
            "Quer um pouco de esporte? Você corre, nós caçamos.",
            "Usaremos seus ossos para marcar o caminho."
        };

		// Combat yells - used during active battles
		private static readonly string[] CombatYell = new string[]
        {
            "Cerquem eles!",
            "Cortem a retirada!",
            "Foquem no conjurador!",
            "Muralha de escudos, mantenham a formação!",
            "Flanqueiem pela esquerda!",
            "Cuidado com emboscadas!",
            "Cubram a retaguarda!",
            "Quebrem a linha deles!",
            "Pressionem o ataque!",
            "Recuem e reorganizem!",
            "Curandeiro caído! Protejam ele!",
            "Estão nos flanqueando!",
            "Emboscada! Armas prontas!",
            "Armadilha! Cuidado onde pisa!",
            "Reforços chegando!",
            "Estamos cercados!",
            "Segurem a linha!",
            "Pela glória!",
            "Resistam e lutem!",
            "Sem retirada!",
            "Acabamos com isso agora!",
            "Lutem ou morram!",
            "Até o último suspiro!",
            "Sem misericórdia!",
            "Dêem aço a eles!",
            "Comigo!",
            "Avancem!",
            "Empurrem eles de volta!",
            "Escudos cerrados!",
            "Segurem esta posição!",
            "Formem fila!",
            "Derrubem eles!",
            "Abatam eles!",
            "Acabem com eles!",
            "Não deixem escapar!",
            "Prendam eles aqui!",
            "Mantenham a pressão!",
            "Olhos abertos!",
            "Espalhem-se!",
            "Fechem as fileiras!",
            "Atacem agora!",
            "Quebrem eles!",
            "Fiquem firmes!",
            "Reúnam-se em mim!",
            "Cortem eles!",
            "Forcem eles a recuar!",
            "Mantenham-se firmes!",
            "Todos de uma vez!",
            "Esmaguem eles!",
            "Acabem com eles!"
        };
        // Post-combat celebration lines
		private static readonly string[] VictoryLines = new string[]
        {
            "Essa foi por pouco! Todo mundo bem?",
            "Boa luta! Verifique o corpo por moedas.",
            "Fazemos um bom time!",
            "Mais um pro chão.",
            "Preciso recuperar o fôlego...",
            "Alguém se feriu gravemente?",
            "Aquela besta foi mais difícil do que esperava.",
            "Vitória! Mas mantenham-se alertas.",
            "Bem lutado, amigos!",
            "*limpa o sangue da arma*",
            "Trabalho em equipe excelente!",
            "Eles não tiveram chance!",
            "Área limpa… por enquanto.",
            "Esse foi o último deles.",
            "Contem as cabeças, certifiquem-se de que ninguém sumiu.",
            "Recuperem o fôlego, mas fiquem de guarda.",
            "Nada mal — ainda estamos vivos.",
            "Limpem suas lâminas, este lugar não é seguro.",
            "Vamos tratar os ferimentos antes de prosseguir.",
            "Bom trabalho, pessoal.",
            "Verifiquem os cantos — emboscada ainda é possível.",
            "Poderia ter sido pior.",
            "Mais um passo em direção às profundezas.",
            "Fiquem atentos, ainda há mais pela frente.",
            "Descanso rápido, depois seguimos.",
            "Luta sólida.",
            "Conquistamos esse momento.",
            "Puta merda, nós sobrevivemos!",
            "Isso vai manter os monstros quietos por um tempo.",
            "Peguem o que precisamos e vamos.",
            "Silêncio agora. Escutem.",
            "Em frente — com cuidado."
        };
        private static readonly string[] RetreatLines = new string[]
        {
            "Recuem! Estou gravemente ferido!",
            "Não aguento mais muito tempo!",
            "Recuando! Cubram-me!",
            "São muitos!",
            "*cambaleia para trás*",
            "Preciso me curar!",
            "Não vou morrer aqui hoje!",
            "Vou cair fora daqui!",
            "Retirada tática!",
            "Recuem, agora!",
            "Rompam contato!",
            "Cubram a retirada!",
            "Eles são mais fortes que nós!",
            "Para trás, para trás!",
            "Recuem para o canto!",
            "Não consigo segurar isso!",
            "Desengajem!",
            "Reagrupem lá fora!",
            "Mexam-se, mexam-se!",
            "Vamos heroicamente dar o fora daqui!",
            "Segurem eles enquanto recuamos!",
            "Essa luta está perdida!",
            "Salvem-se!",
            "Formem em mim e recuem!",
            "Recuem para a cobertura!",
            "Recuemos enquanto podemos!",
            "Não deixem eles nos perseguirem!",
            "Para fora, agora!",
            "Todos recuem!"
        };
		private static readonly string[] PotionLines = new string[]
        {
            "*bebe uma poção de cura*",
            "Isso deve ajudar!",
            "*engole a poção apressadamente*",
            "Muito melhor!",
            "*destampa o frasco*",
            "Ainda bem que trouxe essas!",
            "*toma a poção*",
            "Ah, isso é melhor!",
            "*engole a poção*",
            "Já estou sentindo fazer efeito.",
            "*limpa a boca*",
            "Isso arde ao descer.",
            "Exatamente o que precisava.",
            "*sacode o frasco vazio*",
            "Continua com um gosto horrível.",
            "Isso vai me manter de pé.",
            "*quebra o selo*",
            "Tem gosto de urina, mas resolve o problema.",
            "Nunca saia de casa sem essas.",
            "*expira aliviado*",
            "Isso me estabilizou.",
            "*guarda a garrafa vazia*",
            "Melhor do que sangrar até morrer.",
            "Isso serve.",
            "*faz careta enquanto a magia age*",
            "Não desperdiçando uma gota.",
            "Ainda bem que guardei isso.",
            "Vamos continuar."
        };
        private static readonly string[] BandageLines = new string[]
        {
            "*aplica ataduras*",
            "*enfaixa os ferimentos*",
            "Só preciso estancar o sangramento...",
            "*envolve os machucados*",
            "Estas ataduras vão segurar.",
            "*trata os ferimentos*",
            "*aperta a atadura*",
            "Fique quieto…",
            "*prende o curativo*",
            "Isso deve diminuir o sangramento.",
            "*esticia a atadura*",
            "Não é bonito, mas serve.",
            "*amarra*",
            "Isso vai ter que segurar.",
            "Pressão ajuda.",
            "*examina o ferimento*",
            "Fique de olho enquanto termino isso.",
            "*enrola mais apertado*",
            "Poderia ser pior.",
            "*limpa o sangue*",
            "Isso deve estancar.",
            "*ajusta o curativo*",
            "Me dê um momento.",
            "Ainda dói, mas está melhor.",
            "*testa o membro*",
            "Bom o suficiente para lutar."
        };
        private static readonly string[] HealSpellLines = new string[]
        {
            "*lança magia de cura em aliado*",
            "In Vas Mani! Seja curado!",
            "*canaliza energia de cura*",
            "Que a luz cure tuas feridas!",
            "*canaliza feitiço restaurador*",
            "Fique quieto, eu vou curá-lo!",
            "*invoca uma prece de cura*",
            "Pela luz, seja restaurado!",
            "*clama pela graça divina*",
            "Que a dor se afaste de ti!",
            "*foca magia restauradora*",
            "Suas feridas se fecham agora!",
            "*impõe as mãos sobre aliado*",
            "Que a força retorne a você!",
            "*sussurra um cântico sagrado*",
            "Seja renovado!",
            "*irradia luz curativa*",
            "A luz responde!",
            "*traça sigilos de renovação*",
            "Levante-se e lute!",
            "*canaliza energia sagrada*",
            "Deixe a vida fluir novamente!",
            "*estende a mão brilhante*",
            "Seja reparado!",
            "*completa o rito de cura*",
            "Fique firme — você está curado!"
        };

        private static readonly int FriendlyChatLength = FriendlyChat.Length;
        private static readonly int EvilChatLength = EvilChat.Length;
        private static readonly int CombatYellLength = CombatYell.Length;
        private static readonly int VictoryLinesLength = VictoryLines.Length;
        private static readonly int RetreatLinesLength = RetreatLines.Length;
        private static readonly int PotionLinesLength = PotionLines.Length;
        private static readonly int BandageLinesLength = BandageLines.Length;
        private static readonly int HealSpellLinesLength = HealSpellLines.Length;
        #endregion

        #region Configuration
        private static readonly TimeSpan SpeechThrottle = TimeSpan.FromSeconds(2.0);
        private const int TeamMemberRange = 12;
        private const double HealSelfThreshold = 0.40;
        private const double HealAllyThreshold = 0.50;
        private const double RetreatThreshold = 0.20;
        #endregion

        #region Instance Fields
        private int m_CitizenType;
        private int m_CitizenLevel;
        private bool m_SpawnedBySystem;
        private bool m_IsEvil;
        private bool m_SpawnMounted;
        private int m_TeamId;

        private long m_LastMessageTicks;
        private DateTime m_NextChatTime;
        private DateTime m_NextHealCheck;
        private DateTime m_PendingDeparture;
        
        private bool m_IsLeaving;
        private bool m_IsUsingBandage;
        private bool m_IsRetreating;
        private DateTime m_RetreatResetTime;

        // Skip thinking when idle
        private int m_ThinkSkipCounter;
        private const int ThinkSkipMax = 3;

		private DateTime m_NextThink;

		private class CachedCount
		{
		    public int Count;
		    public DateTime Time;

		    public CachedCount()
		    {
		        Time = DateTime.MinValue;
		    }
		}


		private static Dictionary<PlayerMobile, CachedCount> s_NearbyCountCache = new Dictionary<PlayerMobile, CachedCount>();


        private List<AdventurerTeam> m_MySquad = new List<AdventurerTeam>();
        #endregion

        #region Properties
        [CommandProperty(AccessLevel.Owner)]
        public CitizenClass CitizenClass
        {
            get { return (CitizenClass)m_CitizenType; }
            set { m_CitizenType = (int)value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.Owner)]
        public int CitizenLevel
        {
            get { return m_CitizenLevel; }
            set { m_CitizenLevel = value; InvalidateProperties(); }
        }

        public bool SpawnedBySystem
        {
            get { return m_SpawnedBySystem; }
            set { m_SpawnedBySystem = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsEvil
        {
            get { return m_IsEvil; }
            set { m_IsEvil = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TeamId
        {
            get { return m_TeamId; }
            set { m_TeamId = value; }
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return m_IsEvil; } }
        #endregion

        #region Constructors
        [Constructable]
        public AdventurerTeam() : this(0, false, false) { }

        [Constructable]
        public AdventurerTeam(int teamId, bool isEvil) : this(teamId, isEvil, false) { }

        [Constructable]
        public AdventurerTeam(int teamId, bool isEvil, bool mounted) 
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            if (teamId != 0)
                this.Team = teamId;

            m_TeamId = teamId; 
            m_IsEvil = isEvil;
            m_SpawnedBySystem = (teamId != 0);
            m_SpawnMounted = mounted;

            FightMode = FightMode.Closest; 
            RangePerception = TeamMemberRange; 

            InitStatsAndAppearance();
            EnforceMountState(mounted);

            DateTime now = DateTime.UtcNow;
            m_NextChatTime = now.AddSeconds(Utility.RandomMinMax(10, 30));
            m_PendingDeparture = now.AddMinutes(Utility.RandomMinMax(20, 40));
            m_LastMessageTicks = now.Ticks;
        }

        public AdventurerTeam(Serial serial) : base(serial) { }
        #endregion

        #region Squad Management (OPTIMIZED - Reactive Cleanup)
        public void AddToSquad(AdventurerTeam member)
        {
            if (member != null && member != this && !m_MySquad.Contains(member))
                m_MySquad.Add(member);
        }

        public void RemoveFromSquad(AdventurerTeam member)
        {
            m_MySquad.Remove(member);
        }

        public override void OnDelete()
        {
            base.OnDelete();
            
            // Notify squad members 
            for (int i = 0; i < m_MySquad.Count; i++)
            {
                if (m_MySquad[i] != null && !m_MySquad[i].Deleted)
                    m_MySquad[i].RemoveFromSquad(this);
            }
            m_MySquad.Clear();
        }
        #endregion

        #region Core Logic
        public override void OnThink()
        {
			
            if (DateTime.UtcNow < m_NextThink)
		       return;
		
	    	m_NextThink = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
		
		    base.OnThink();
            
			if (Deleted || Map == null || Map == Map.Internal) return;

            // Skip thinking every 3rd cycle when idle
            if (Combatant == null && !m_IsRetreating)
            {
                if (++m_ThinkSkipCounter < ThinkSkipMax)
                    return;
                m_ThinkSkipCounter = 0;
            }

            DateTime now = DateTime.UtcNow;

            if (m_IsRetreating)
            {
                if (now > m_RetreatResetTime)
                    m_IsRetreating = false;
                else
                {
                    Combatant = null;
                    Warmode = false;
                    return;
                }
            }

            if (!m_IsLeaving && m_TeamId != 0 && now > m_PendingDeparture)
            {
                if (Combatant == null && Utility.RandomDouble() < 0.10)
                {
                    m_IsLeaving = true;
                    if (CanSendMessage(now)) Say("Hora de partir."); 
                    Timer.DelayCall(TimeSpan.FromSeconds(5.0), ExecuteDeparture);
                }
            }

            if (now > m_NextHealCheck)
            {
                m_NextHealCheck = now + TimeSpan.FromSeconds(6);
                PerformCombatSupport(now);
            }

            if (Combatant == null && now > m_NextChatTime)
            {
                if (CanSendMessage(now))
                {
                    Say(m_IsEvil ? 
                        GetPooledMessage(EvilChat, EvilChatLength) : 
                        GetPooledMessage(FriendlyChat, FriendlyChatLength));
                    m_NextChatTime = now + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 45));
                }
            }
			// dictionary clean up
			if (Utility.Random(100) == 0)
			    PruneNearbyCache();
        }

        private void PerformCombatSupport(DateTime now)
        {
            double hpRatio = (double)Hits / HitsMax;
            
            if (hpRatio < HealSelfThreshold)
                TryHealSelf(now);

            if (m_CitizenType == (int)CitizenClass.Wizard && Mana > 10)
            {
                // handled by OnDelete
                for (int i = 0; i < m_MySquad.Count; i++)
                {
                    AdventurerTeam ally = m_MySquad[i];
                    
                    if (ally.Alive && ally.Map == this.Map && ally.InRange(this, 12))
                    {
                        if (ally.Hits < (ally.HitsMax * HealAllyThreshold))
                        {
                            DoMagicHeal(ally);
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Combat Events & Retreat
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
            
            if (willKill || Deleted) return;

            if (!m_IsRetreating && m_CitizenType != (int)CitizenClass.Fighter)
            {
                double hpRatio = (double)Hits / HitsMax;
                if (hpRatio < RetreatThreshold && Utility.RandomDouble() < 0.35)
                {
                    m_IsRetreating = true;
                    m_RetreatResetTime = DateTime.UtcNow + TimeSpan.FromSeconds(6.0);
                    
                    if (CanSendMessage(DateTime.UtcNow)) 
                        Say(GetPooledMessage(RetreatLines, RetreatLinesLength));
                    
                    Combatant = null;
                    Warmode = false;
                }
            }
        }

        public override void OnCombatantChange()
        {
            base.OnCombatantChange();
            DateTime now = DateTime.UtcNow;

            if (Combatant != null)
            {
                if (CanSendMessage(now) && Utility.RandomDouble() < 0.3)
                    Say(GetPooledMessage(CombatYell, CombatYellLength));
            }
            else
            {
                if (!m_IsRetreating && CanSendMessage(now) && Utility.RandomDouble() < 0.5)
                    Say(GetPooledMessage(VictoryLines, VictoryLinesLength));
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_IsLeaving || Deleted || m == null || !m.Alive || !m.Player) return;

            if (CanSendMessage(DateTime.UtcNow) && CanSee(m) && m.InRange(this, 8))
            {
                Say(m_IsEvil ? 
                    GetPooledMessage(EvilChat, EvilChatLength) : 
                    GetPooledMessage(FriendlyChat, FriendlyChatLength));
            }
        }
        #endregion

        #region Actions (Heal/Support)
        private void TryHealSelf(DateTime now)
        {
            BaseHealPotion potion = Backpack.FindItemByType(typeof(BaseHealPotion)) as BaseHealPotion;
            if (potion != null)
            {
                potion.Drink(this);
                PublicOverheadMessage(MessageType.Emote, 0x3B2, true, 
                    GetPooledMessage(PotionLines, PotionLinesLength));
                return;
            }

            if (m_CitizenType != (int)CitizenClass.Wizard && !m_IsUsingBandage)
            {
                Bandage bandage = Backpack.FindItemByType(typeof(Bandage)) as Bandage;
                if (bandage != null)
                {
                    m_IsUsingBandage = true;
                    bandage.Consume(1);
                    PublicOverheadMessage(MessageType.Emote, 0x3B2, true, 
                        GetPooledMessage(BandageLines, BandageLinesLength));
                    
                    Timer.DelayCall(TimeSpan.FromSeconds(4.0), delegate 
                    { 
                        if (!Deleted && Alive) 
                        {
                            Heal(Utility.RandomMinMax(20, 40)); 
                            PlaySound(0x57);
                            m_IsUsingBandage = false;
                        }
                    });
                }
            }
            else if (m_CitizenType == (int)CitizenClass.Wizard)
            {
                DoMagicHeal(this);
            }
        }

        private void DoMagicHeal(Mobile target)
        {
            if (Mana < 10) return;
            Mana -= 10;
            if (target != this) Direction = GetDirectionTo(target);
            
            Animate(17, 7, 1, true, false, 0);
            PlaySound(0x1F2);
            PublicOverheadMessage(MessageType.Emote, 0x3B2, true, 
                GetPooledMessage(HealSpellLines, HealSpellLinesLength));
            
            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
            {
                if (!Deleted && Alive && target.Alive && target.Map == Map && target.InRange(this, 12))
                {
                    target.Heal(Utility.RandomMinMax(20, 35));
                    target.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                }
            });
        }
        #endregion

        #region Setup (Mounts & Appearance)
        private void EnforceMountState(bool shouldBeMounted)
        {
            if (shouldBeMounted &&  Server.Mobiles.AnimalTrainer.IsNoMountRegion( this, this.Region ))
            {
                if (this.Mount == null)
                    new Horse().Rider = this;
            }
            else
            {
                if (this.Mount != null)
                {
                    IMount mount = this.Mount;
                    mount.Rider = null;
                    if (mount is Mobile) ((Mobile)mount).Delete();
                }
            }
        }

        private void InitStatsAndAppearance()
        {
            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            Name = Female ? NameList.RandomName("female") : NameList.RandomName("male");

            if (!Female)
                FacialHairItemID = Utility.RandomList(0, 8254, 8255, 8256, 8257, 8267, 8268, 8269);

            m_CitizenLevel = Utility.RandomMinMax(1, 9);
            Fame = 2500 * m_CitizenLevel;
            Karma = m_IsEvil ? -Fame : Fame;
            VirtualArmor = m_CitizenLevel * 10;
            
            if (m_IsEvil)
            {
                Title = TavernPatrons.GetEvilTitle();
                Hue = Utility.RandomList(0x995, 0x8A4, 0x8B0, 0x8AC);
                FightMode = FightMode.Closest;
            }
            else
            {
                Title = TavernPatrons.GetTitle();
                Hue = Utility.RandomSkinHue();
                FightMode = FightMode.Evil;
            }

            Utility.AssignRandomHair(this);
            SpeechHue = Utility.RandomTalkHue();
            HairHue = FacialHairHue = Utility.RandomHairHue();

            int baseSkill = 25 + (m_CitizenLevel * 10);
            int strMin = m_CitizenLevel * 20, strMax = m_CitizenLevel * 30;
            int dexMin = m_CitizenLevel * 20, dexMax = m_CitizenLevel * 30;
            int intMin = m_CitizenLevel * 20, intMax = m_CitizenLevel * 30;
            int hitsMin = m_CitizenLevel * 30, hitsMax = m_CitizenLevel * 40;
            
            int type = Utility.Random(3);
            switch (type)
			{
				case 0: // Wizard
					IntelligentAction.DressUpWizards(this, m_IsEvil);
					m_CitizenType = (int)CitizenClass.Wizard;
					AI = AIType.AI_Mage;
					
					SetSkill(SkillName.Psychology, baseSkill);
					SetSkill(SkillName.Magery, baseSkill);
					SetSkill(SkillName.Meditation, baseSkill);
					SetSkill(SkillName.MagicResist, baseSkill);
					SetSkill(SkillName.FistFighting, baseSkill);
					SetSkill(SkillName.Tactics, baseSkill - 20);
					
					intMax += m_CitizenLevel * 30;
					break;
					
				case 1: // Fighter
					IntelligentAction.DressUpFighters(this, "", m_IsEvil, false, true);
					m_CitizenType = (int)CitizenClass.Fighter;
					AI = AIType.AI_Melee;
					
					SetSkill(SkillName.Fencing, baseSkill);
					SetSkill(SkillName.Bludgeoning, baseSkill);
					SetSkill(SkillName.Swords, baseSkill);
					SetSkill(SkillName.Parry, baseSkill);
					SetSkill(SkillName.MagicResist, baseSkill);
					SetSkill(SkillName.Tactics, baseSkill + 10);
					SetSkill(SkillName.Healing, baseSkill + 10);
					SetSkill(SkillName.Anatomy, baseSkill);
					
					strMax += m_CitizenLevel * 10;
					hitsMax += m_CitizenLevel * 20;
					break;
					
				case 2: // Rogue
					IntelligentAction.DressUpRogues(this, "", m_IsEvil, false, true);
					m_CitizenType = (int)CitizenClass.Rogue;
					AI = AIType.AI_Archer;
					
					SetSkill(SkillName.Marksmanship, baseSkill);
					SetSkill(SkillName.Tactics, baseSkill);
					SetSkill(SkillName.MagicResist, baseSkill);
					SetSkill(SkillName.Healing, baseSkill);
					SetSkill(SkillName.Anatomy, baseSkill - 10);
					
					dexMax += m_CitizenLevel * 10;
					break;
			}

            SetStr(strMin, strMax);
            SetDex(dexMin, dexMax);
            SetInt(intMin, intMax);
            SetHits(hitsMin, hitsMax);
            
            AddWeapon(true);
            AddHealingSupplies();
        }

		public void AddWeapon(bool initial)
		{
			BaseWeapon hand = FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
			BaseWeapon twohand = FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

			if (!initial && (hand != null || twohand != null))
				return;

			if (m_CitizenType == (int)CitizenClass.Fighter)
			{
				if (hand != null || twohand != null)
					return;
				
				switch (Utility.Random(3))
				{
					case 0: AddItem(new Longsword()); break;
					case 1: AddItem(new BattleAxe()); break;
					case 2: AddItem(new Mace()); break;
				}
				return;
			}

			if (initial)
			{
				if (hand != null) hand.Delete();
				if (twohand != null) twohand.Delete();
			}

			if (Utility.RandomBool() && (m_CitizenType == (int)CitizenClass.Wizard || m_CitizenType == (int)CitizenClass.Rogue))
			{
				Item glove = new Item(0x13c6);
				glove.Name = "Throwing Gloves";
				AddItem(glove);
				
				Item ammo = new Item(0xF0E);
				ammo.Name = "Throwing Ammunition";
				PackItem(ammo);
				return;
			}

			if (m_CitizenType == (int)CitizenClass.Wizard)
			{
				if (Utility.RandomBool())
					AddItem(new GnarledStaff());
				else
					AddItem(new QuarterStaff());
				return;
			}

			if (m_CitizenType == (int)CitizenClass.Rogue)
			{
				switch (Utility.Random(8))
				{
					case 0: AddItem(new Bow()); PackItem(new Arrow(Utility.RandomMinMax(20, 40))); break;
					case 1: AddItem(new Crossbow()); PackItem(new Bolt(Utility.RandomMinMax(20, 40))); break;
					case 2: AddItem(new HeavyCrossbow()); PackItem(new Bolt(Utility.RandomMinMax(20, 40))); break;
					case 3: AddItem(new RepeatingCrossbow()); PackItem(new Bolt(Utility.RandomMinMax(20, 40))); break;
					case 4: AddItem(new CompositeBow()); PackItem(new Arrow(Utility.RandomMinMax(20, 40))); break;
					case 5: AddItem(new Bow()); PackItem(new Arrow(Utility.RandomMinMax(20, 40))); break;
					case 6: AddItem(new Crossbow()); PackItem(new Bolt(Utility.RandomMinMax(20, 40))); break;
					case 7: AddItem(new Crossbow()); PackItem(new Bolt(Utility.RandomMinMax(20, 40))); break;
				}
				return;
			}
		}

		private void AddHealingSupplies()
		{
			int potionCount = Utility.RandomMinMax(3, 5);
			int bandageCount = Utility.RandomMinMax(20, 40);
			
			switch (m_CitizenType)
			{
				case (int)CitizenClass.Wizard:
					if (m_CitizenLevel >= 7)
					{
						for (int i = 0; i < Math.Max(2, potionCount - 2); i++)
                        {
							PackItem(new GreaterHealPotion());
                        }
                        if(Utility.RandomBool())
                        {
                            int roll;
                            roll = Utility.Random(4);
                            switch (roll)
                            {
                                case 0:
                                   PackItem(new GreaterManaPotion());
                                    break;
                                case 1:
                                    PackItem(new ArcaneGem());
                                    break;
                                case 2:
                                    PackItem(new PotionOfWisdom());
                                    break;
                                case 3:
                                    PackItem(new ManaPotion());
                                    break;
                            }
                        }
					}
					else if (m_CitizenLevel >= 4)
					{
						for (int i = 0; i < Math.Max(2, potionCount - 2); i++)
                        {
                            PackItem(new HealPotion());
                        }
                        if(Utility.RandomBool())
                        {
                            int roll;
                            roll = Utility.Random(3);
                            switch (roll)
                            {
                                case 0:
                                   PackItem(new GreaterManaPotion());
                                    break;
                                case 1:
                                    PackItem(new ArcaneGem());
                                    break;
                                case 2:
                                    PackItem(new ManaPotion());
                                    break;
                            }
                        }
					}
					else
					{
						for (int i = 0; i < Math.Max(2, potionCount - 2); i++)
							PackItem(new LesserHealPotion());
					}
					break;
					
				case (int)CitizenClass.Fighter:
					PackItem(new Bandage(bandageCount + 10));
					
					if (m_CitizenLevel >= 7)
					{
						for (int i = 0; i < potionCount; i++)
                        {
							PackItem(new GreaterHealPotion());
                        }
                        if(Utility.RandomBool())
                        {
                            int roll;
                            roll = Utility.Random(4);
                            switch (roll)
                            {
                                case 0:
                                   PackItem(new GreaterCurePotion());
                                    break;
                                case 1:
                                    PackItem(new GreaterHealPotion());
                                    break;
                                case 2:
                                    PackItem(new PotionOfMight());
                                    break;
                                case 3:
                                    PackItem(new CurePotion());
                                    break;
                            }
                        }
					}
					else if (m_CitizenLevel >= 4)
					{
						for (int i = 0; i < potionCount; i++)
                        {
							PackItem(new HealPotion());
                        }
                        if(Utility.RandomBool())
                        {
                            int roll;
                            roll = Utility.Random(3);
                            switch (roll)
                            {
                                case 0:
                                   PackItem(new GreaterCurePotion());
                                    break;
                                case 1:
                                    PackItem(new GreaterHealPotion());
                                    break;
                                case 2:
                                    PackItem(new CurePotion());
                                    break;
                            }
                        }

					}
					else
					{
						for (int i = 0; i < potionCount; i++)
							PackItem(new LesserHealPotion());
					}
					break;
					
				case (int)CitizenClass.Rogue:
					PackItem(new Bandage(bandageCount));
					
					if (m_CitizenLevel >= 7)
					{
						for (int i = 0; i < potionCount; i++)
                        {
							PackItem(new GreaterHealPotion());
                        }
                        if(Utility.RandomBool())
                        {
                            int roll;
                            roll = Utility.Random(4);
                            switch (roll)
                            {
                                case 0:
                                   PackItem(new GreaterCurePotion());
                                    break;
                                case 1:
                                    PackItem(new MasterSkeletonsKey());
                                    break;
                                case 2:
                                    PackItem(new PotionOfDexterity());
                                    break;
                                case 3:
                                    PackItem(new CurePotion());
                                    break;
                            }
                        }

					}
					else if (m_CitizenLevel >= 4)
					{
						for (int i = 0; i < potionCount; i++)
                        {
							PackItem(new HealPotion());
                        }
                        if(Utility.RandomBool())
                        {
                            int roll;
                            roll = Utility.Random(3);
                            switch (roll)
                            {
                                case 0:
                                   PackItem(new GreaterCurePotion());
                                    break;
                                case 1:
                                    PackItem(new MasterSkeletonsKey());
                                    break;
                                case 2:
                                    PackItem(new CurePotion());
                                    break;
                            }
                        }

					}
					else
					{
						for (int i = 0; i < potionCount; i++)
							PackItem(new LesserHealPotion());
					}
					break;
			}
		}

        #endregion

        #region Helpers

		private static int ComputeNearbyCount(PlayerMobile pm)
		{
		    if (pm == null || pm.Deleted || pm.Map == null)
		        return 0;

		    int count = 0;

		    foreach (Mobile m in pm.GetMobilesInRange(8))
		    {
		        if (m != null && !m.Deleted && m is BaseCreature)
		            count++;
		    }

		    return count;
		}

		private static void PruneNearbyCache()
		{
		    List<PlayerMobile> remove = null;
		    DateTime now = DateTime.UtcNow;

		    foreach (KeyValuePair<PlayerMobile, CachedCount> kv in s_NearbyCountCache)
		    {
		        if (kv.Key == null || kv.Key.Deleted || now - kv.Value.Time > TimeSpan.FromMinutes(5))
		        {
		            if (remove == null)
		                remove = new List<PlayerMobile>();

		            remove.Add(kv.Key);
		        }
		    }

		    if (remove != null)
		        for (int i = 0; i < remove.Count; i++)
		            s_NearbyCountCache.Remove(remove[i]);
		}


        private bool CanSendMessage(DateTime now)
        {
            long nowTicks = now.Ticks;
            if ((nowTicks - m_LastMessageTicks) < SpeechThrottle.Ticks) 
                return false;
            m_LastMessageTicks = nowTicks;
            return true;
        }

        private string GetPooledMessage(string[] source, int cachedLength)
        {
            if (source == null || cachedLength == 0) return "";
            return source[Utility.Random(cachedLength)];
        }

        private void ExecuteDeparture()
        {
            if (m_TeamId != 0) AutoTeamMaintainer.RecycleTeamId(m_TeamId);
            Delete();
        }

        public override void GenerateLoot()
        {
            if (m_CitizenLevel >= 7) AddLoot(LootPack.Rich);
            else if (m_CitizenLevel >= 5) AddLoot(LootPack.Average);
            else AddLoot(LootPack.Meager);
			
            if (Utility.Random(25) == 0)
            {
                Type rareType = Loot.AdventurerRareItemTypes[Utility.Random(Loot.AdventurerRareItemTypes.Length)];
                Item rare = Activator.CreateInstance(rareType) as Item;
                if (rare != null) PackItem(rare);
            }

            if (m_CitizenType == (int)CitizenClass.Wizard)
                AddLoot(LootPack.MedScrolls, (m_CitizenLevel / 3) + 1);
        }

        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 
            writer.Write((int)0); 
        }
        
        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 
            int v = reader.ReadInt(); 
            Timer.DelayCall(TimeSpan.Zero, Delete); 
        }
        #endregion
    }
    // ========================================================================
    // AUTO TEAM MAINTAINER 
    // ========================================================================
    public static class AutoTeamMaintainer
    {
        private static bool s_Enabled = true;
        private static Timer s_MaintenanceTimer;
        private static readonly Queue<int> s_RecycledIds = new Queue<int>();
        private static int s_NextTeamId = 1;
        private static readonly object s_IdLock = new object();

        //Track last spawn attempt per player
        private static readonly Dictionary<Mobile, DateTime> s_LastSpawnAttempt = new Dictionary<Mobile, DateTime>();
        private static readonly TimeSpan SpawnCooldown = TimeSpan.FromMinutes(4);
        
        // Cache nearby counts
        private static readonly Dictionary<Mobile, CachedCount> s_NearbyCountCache = new Dictionary<Mobile, CachedCount>();
		private static readonly TimeSpan CountCacheDuration = TimeSpan.FromSeconds(5);


        private class CachedCount
        {
            public int Count;
            public DateTime Expires;
			 public DateTime Time;

        }

        public static void Initialize()
        {
            if (s_MaintenanceTimer != null) s_MaintenanceTimer.Stop();
            s_MaintenanceTimer = Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), MaintainTeams);
        }

        public static int GetNewTeamId()
        {
            lock (s_IdLock) 
                return (s_RecycledIds.Count > 0) ? s_RecycledIds.Dequeue() : s_NextTeamId++;
        }

        public static void RecycleTeamId(int id)
        {
            lock (s_IdLock) 
                s_RecycledIds.Enqueue(id);
        }

        private static void MaintainTeams()
        {
            if (!s_Enabled) return;
            
            List<Mobile> eligiblePlayers = new List<Mobile>();
            DateTime now = DateTime.UtcNow;
            
            foreach (NetState state in NetState.Instances)
            {
                Mobile m = state.Mobile;
                if (m != null && m.Player && m.Alive && m.Map != null && m.Map != Map.Internal)
                {
					DateTime lastAttempt;
                    if (s_LastSpawnAttempt.TryGetValue(m, out lastAttempt))
                    {
                        if (now - lastAttempt < SpawnCooldown)
                            continue;
                    }
                    
                    eligiblePlayers.Add(m);
                }
            }
            
            List<Mobile> toRemove = new List<Mobile>();
            foreach (var kvp in s_LastSpawnAttempt)
            {
                if (kvp.Key.Deleted || kvp.Key.NetState == null)
                    toRemove.Add(kvp.Key);
            }
            foreach (var m in toRemove)
            {
                s_LastSpawnAttempt.Remove(m);
                s_NearbyCountCache.Remove(m);
            }
            
            // max 5 spawns attempts regardless of how many online players
            int maxProcessPerCycle = Math.Min(5, eligiblePlayers.Count);
            for (int i = 0; i < maxProcessPerCycle; i++)
            {
                Mobile pm = eligiblePlayers[Utility.Random(eligiblePlayers.Count)];
                s_LastSpawnAttempt[pm] = now;
                
                if (Utility.RandomDouble() < 0.1)
                    TrySpawnTeamForPlayer(pm);
            }
        }

        private static void TrySpawnTeamForPlayer(Mobile pm)
		{
		    DateTime now = DateTime.UtcNow;
		
		    CachedCount cached;
		    int nearbyCount;
		
		    if (!s_NearbyCountCache.TryGetValue(pm, out cached))
		    {
		        cached = new CachedCount();
		        s_NearbyCountCache[pm] = cached;
		        cached.Time = DateTime.MinValue;
		    }
		
		    if (now - cached.Time < CountCacheDuration)
		    {
		        nearbyCount = cached.Count;
		    }
		    else
		    {
		        nearbyCount = ComputeNearbyCount(pm);
		        cached.Count = nearbyCount;
		        cached.Time = now;
		    }
		
		    if (nearbyCount > 5)
		        return;
		
		    // reset cache after spawn
		    s_NearbyCountCache.Remove(pm);
		
		    Point3D spawnLoc = FindSpawnLocation(pm);
		    if (spawnLoc == Point3D.Zero)
		        return;
		
		    int teamId = GetNewTeamId();
		    bool isEvil = Utility.RandomBool();
		    bool mounted = Utility.RandomDouble() < 0.4;
		    int size = Utility.RandomMinMax(3, 6);
		
		    List<AdventurerTeam> newSquadMembers = new List<AdventurerTeam>();
		
		    for (int i = 0; i < size; i++)
		    {
		        AdventurerTeam npc = new AdventurerTeam(teamId, isEvil, mounted);
		        npc.MoveToWorld(spawnLoc, pm.Map);
		        newSquadMembers.Add(npc);
		    }
		
		    for (int i = 0; i < newSquadMembers.Count; i++)
		    {
		        AdventurerTeam member = newSquadMembers[i];
		        for (int j = 0; j < newSquadMembers.Count; j++)
		        {
		            if (member != newSquadMembers[j])
		                member.AddToSquad(newSquadMembers[j]);
		        }
		    }
		}


		private static int ComputeNearbyCount(Mobile pm)
		{
		    if (pm == null || pm.Deleted || pm.Map == null)
		        return 0;

		    int count = 0;

		    foreach (Mobile m in pm.GetMobilesInRange(12))
		    {
		        if (m != null && !m.Deleted && m is AdventurerTeam)
		            count++;
		    }

		    return count;
		}

        private static Point3D FindSpawnLocation(Mobile nearPlayer)
        {
            Map map = nearPlayer.Map;
            
            // Try closest distances first
            int[] distances = { 24, 28, 32, 35 };
            
            for (int d = 0; d < distances.Length; d++)
            {
                int dist = distances[d];
                
                for (int a = 0; a < 2; a++)
                {
                    double ang = Utility.RandomDouble() * Math.PI * 2;
                    int x = nearPlayer.X + (int)(Math.Cos(ang) * dist);
                    int y = nearPlayer.Y + (int)(Math.Sin(ang) * dist);
                    Point3D p = new Point3D(x, y, map.GetAverageZ(x, y));

                    if (IsInForbiddenRegion(p, map)) continue;
                    if (map.CanSpawnMobile(p)) return p;
                }
            }
            return Point3D.Zero;
        }

        private static bool IsInForbiddenRegion(Point3D loc, Map map)
        {
            Region reg = Region.Find(loc, map);
            if (reg == null) return false;

            if (reg is WantedRegion || reg is SavageRegion || reg is VillageRegion ||
                reg is UnderHouseRegion || reg is UmbraRegion || reg is TownRegion ||
                reg is StartRegion || reg is SkyHomeDwelling || reg is SafeRegion ||
                reg is ProtectedRegion || reg is PublicRegion || reg is PirateRegion ||
                reg is BardTownRegion || reg is DawnRegion || reg is DungeonHomeRegion ||
                reg is GargoyleRegion || reg is GuardedRegion || reg is HouseRegion ||
                reg is LunaRegion || reg is MazeRegion || reg is MoonCore)
            {
                return true;
            }
            return false;
        }
    }
}