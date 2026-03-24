using System;

namespace Server.Custom.Ascensions
{
    public static class AscensionDefinitions
    {
        public static string GetDescription(AscensionType type)
        {
            switch (type)
            {
                case AscensionType.Berserker:
                    return
                    "O Berserker é um guerreiro selvagem que empunha armas de duas mãos com grande efeito. Em combate, eles entram em uma fúria assassina que os fortalece.<br>" +
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Táticas, Acampamento e Resistência Mágica. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>" +
                    "Então, um Berserker de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Táticas, Acampamento e Resistência Mágica.<br>"+
                    "Berserkers também exigem que seu espírito esteja livre da mancha da civilização - um Berserker não ganhará experiência nesta Ascensão se tiver aprendido Bushido, Cavalaria, Magia ou Necromancia.";
                case AscensionType.Archmage:
                    return
                    "O Archmage é um mestre do Arcano. Em combate, eles controlam seus oponentes e executam feitiços poderosos para dobrar o próprio Tecido à sua vontade.<br>" +
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Magia, Inscrição e Psicologia. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>" +
                    "Então, um Archmage de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Magia, Inscrição e Psicologia.<br>"+
                    "Archmages também exigem um foco singular na magia arcana, e não ganharão experiência nesta classe se tiverem aprendido Cavalaria, Elementalismo, Necromancia ou Bushido.";
                case AscensionType.Palemaster:
                    return
                    "O Palemaster é a morte em carne viva. Em combate, eles controlam um exército de mortos-vivos e amaldiçoam e enfraquecem seus oponentes até que encontrem seu fim.<br>" +
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Necromancia, Forense e Espiritualismo. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>" +
                    "Então, um Palemaster de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Necromancia, Forense e Espiritualismo.<br>"+
                    "Palemasters também são obrigados a se deleitar na morte e na vileza, e não ganharão experiência nesta classe se tiverem aprendido Cavalaria, Elementalismo ou Bushido, ou se desviarem do caminho do mal.";
                case AscensionType.Crusader:
                    return
                    "O Crusader é a personificação da virtude. Em combate, eles se mantêm firmes em desafio ao mal.<br><br>"+
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Cavalaria, Espiritualismo e Táticas. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>"+
                    "Então, um Crusader de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Cavalaria, Espiritualismo e Táticas.<br>"+
                    "Crusaders também são obrigados a seguir o caminho da justiça e defender o bem, e não ganharão experiência nesta classe se tiverem aprendido Bushido, Necromancia ou Forense, ou se desviarem do caminho do bem.<br>";
                case AscensionType.Assassin:
                    return
                    "O Assassin é um especialista em encerrar vidas prematuramente. Em combate, eles empregam venenos com grande efeito para prejudicar e aniquilar seus inimigos.<br>"+
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Envenenamento, Furtividade e Esgrima. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>"+
                    "Então, um Assassin de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Envenenamento, Furtividade e Esgrima.<br>"+
                    "Assassins também são obrigados a abandonar sua moral, e não ganharão experiência nesta classe se tiverem aprendido Bushido ou Cavalaria, ou se abandonarem o caminho do mal.";   
                case AscensionType.Blackguard:
                    return
                    "O Blackguard é um campeão do mal e da corrupção. Em combate, eles são verdadeiros tanques que trazem gelo e sangue ao campo de batalha.<br>"+
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Cavalaria, Conhecimento de Armas e Necromancia. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>"+
                    "Então, um Blackguard de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Cavalaria, Conhecimento de Armas e Necromancia.<br>"+
                    "Blackguards também são obrigados a abandonar sua moral, e não ganharão experiência nesta classe se tiverem aprendido Bushido, Espiritualismo ou Elementalismo, ou se abandonarem o caminho do mal.";
                case AscensionType.Skald:
                    return 
                    "O Skald é o herói da canção e o guardião do valor. Em combate, eles usam música poderosa para fortalecer a si mesmos e a seus aliados.<br>"+
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Música, Táticas e Discórdia. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>"+
                    "Então, um Skald de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Música, Táticas e Discórdia.<br>"+
                    "Skalds são orgulhosos e pragmáticos. Eles não ganharão experiência nesta classe se tiverem aprendido Bushido, Necromancia, Pontaria, Furtividade ou Mendicância.";
                case AscensionType.Reaver:
                    return 
                    "O Reaver é um combatente vicioso e selvagem. Em combate, eles se especializam em fazer seus oponentes sangrarem e sugar sua vontade de viver com golpes poderosos e incapacitantes.<br>"+
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Táticas, Anatomia e Forense. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>"+
                    "Então, um Reaver de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Táticas, Anatomia e Forense.<br>"+
                    "Reavers não ganharão experiência nesta classe se tiverem aprendido Bushido, Cura, Veterinária ou Espiritualismo.";
                case AscensionType.Kensai:
                    return
                    "O Kensai é mestre da lâmina. Em combate, eles se especializam em golpes poderosos e pura maestria do Bushido.<br>"+
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Bushido, Conhecimento de Armas e Esgrima. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>"+
                    "Então, um Kensai de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Bushido, Conhecimento de Armas e Esgrima.<br>"+
                    "Kensai não ganharão experiência nesta classe se tiverem aprendido Cavalaria, Magia, Necromancia ou Ninjitsu.";
                case AscensionType.Hierophant:
                    return 
                    "O Hierophant é mestre do divino. Em combate, eles se especializam em conjurar auxílio de sua divindade e canalizar poder divino para ajudar seus aliados.<br>"+
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Cura, Espiritualismo e Meditação. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>"+
                    "Então, um Hierophant de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Cura, Espiritualismo e Meditação.<br>"+
                    "Hierophants não ganharão experiência nesta classe se tiverem aprendido Cavalaria, Forense, Necromancia, Bushido ou Ninjitsu.";
                case AscensionType.ArcaneArcher:
                    return 
                    "O Arcane Archer é mestre atirador e conjurador. Em combate, eles se especializam em chover projéteis mágicos sobre seus oponentes.<br>"+
                    "Para ativar esta Ascensão, você precisa ter 95 de habilidade base em Magia, Foco, Inscrição e Pontaria. Cada vez que você sobe de nível na classe, o requisito também aumenta em 1.<br>"+
                    "Então, um Arcane Archer de nível 20 não pode ativar esta ascensão a menos que tenha 115 de habilidade base em Magia, Foco, Inscrição e Pontaria.<br>"+
                    "Arcane Archers não ganharão experiência nesta classe se tiverem aprendido Cavalaria, Necromancia, Elementalismo ou Bushido.";
                default:
                    return "EM DESENVOLVIMENTO.";
            }
        }


        public static string GetAbilities(AscensionType type)
        {
            if(type == AscensionType.Berserker)
            {
                return
                "<BASEFONT COLOR=#FFFFFF>" +
                "Estas são as habilidades que o berserker aprende ao subir de nível:<br><br>" + 

                "Rage, Nível 1<br>" +
                "Comando: [BerserkerRage<br>" +
                "Ganha +(15+nível) FOR, imunidade a paralisia, +10% de dano, -10% de chance de defesa.<br>" +
                "Dura 10 + nível segundos. 1 min de recarga. Termina com 50% de perda de vigor.<br>" +
                "Nvl 10: +nível/2 de resistências, +5% de dano, -5% de chance de defesa.<br>" +
                "Nvl 15: Golpes acima de 40% da vida máxima são reduzidos para 40%.<br>" +
                "Nvl 20: +5% adicional de dano, -5% de chance de defesa.<br><br>" +   

                "Leap Slam, Nível 6<br>" +
                "Comando: [BerserkerLeapSlam<br>" +
                "Salta 3 + nível/4 tiles. Ao aterrissar, causa 20–35 + ((FOR/15)+(nivel/3)) de dano aos inimigos adjacentes.<br>" +
                "Recarga: 9 seg - (1 seg a cada 3 níveis).<br>" +
                "Nvl 12: Atordoa inimigos adjacentes por 3 seg.<br><br>" +   

                "Warcry, Nível 11<br>" +
                "Comando: [BerserkerWarCry<br>" +
                "Inimigos perdem (25+nível) FOR, (10+nível) DES, (10+nível) INT por (20+nível) seg.<br>" +
                "Aliados ganham 2 de vigor a cada 2 seg pela mesma duração.<br>" +
                "Nvl 16: Inimigos perdem 20+2*nível de vigor instantaneamente e 2+nível a cada 2 seg.<br><br>" +  

                "Tenacity, Nível 18<br>" +
                "Comando: [BerserkerTenacity<br>" +
                "Cura completamente a vida imediatamente e novamente a cada 10 seg por 30 seg. 2 min de recarga.<br>" +
                "Nvl 20: Também cura completamente o vigor a cada pulso.<br><br>" + 

                "Passivas:<br>" +
                "Cleave, nível 2<br>" +
                "Ao realizar um golpe corpo a corpo com uma arma de duas mãos, o berserker tem 4% + 1% por nível de chance de realizar outro golpe contra um inimigo adjacente.<br>" +
                "No nível 5, eles têm 1%+1%/nível de chance de um segundo golpe adicional. No nível 13, eles têm 1%+1%/nível de chance de um terceiro golpe adicional.<br><br>" +    

                "Uncanny dodge, nível 8<br>" +
                "O berserker tem 25+1%/nível de chance de ignorar os efeitos de armadilhas.<br>"+
                "Nível 17: a chance aumenta para 45+1%/nível.<br><br>"+

                "Pummeling Strikes, nível 14<br>" +
                "Ao realizar um golpe com uma arma corpo a corpo de duas mãos, o berserker tem (nível/4)% de chance de ignorar a armadura do alvo.<br>"+
                "Nível 19: a chance aumenta para (nível/2)%<br><br>"+

                "Undying Wrath, nível 20<br>" +
                "Se o berserker sofrer um golpe que reduziria seus pontos de vida a 0 ou menos, ele reduz para 1 em vez disso e a recarga de seu Warcry é zerada.<br>"+
                "Este efeito pode ocorrer uma vez por minuto.<br>"+
                "</BASEFONT>";                
            }
            else if ( type == AscensionType.Archmage)
            {
                return 
                "<BASEFONT COLOR=#FFFFFF>" +
                "Estas são as habilidades que o Archmage aprende ao subir de nível:<br><br>" + 

                "Arcane storm, nível 1<br>" +
                "comando: [ArchmageArcaneStorm <br>" +
                "O archmage libera (6 + 1 a cada 3 níveis de archmage) mísseis de energia no alvo, cada míssil causa 10-18 + (int/25 + nível/3) de dano.<br>" + 
                "Lançar este feitiço custa 45 de mana, e uma vez usado, não pode ser usado novamente por (45-1/nível) segundos.<br>" +
                "nível 10: um alvo atingido por uma tempestade arcana tem 3% de chance por nível do archmage de receber um debuff de -(8 + nível/4) de resistência a energia.<br>" + 
                "que dura 12 + nível do archmage /2 segundos.<br>" + 
                "nível 15: um alvo atingido por uma tempestade arcana perde 34 + nível/2 de vigor após ser atingido por uma tempestade de mísseis.<br>" + 
                "nível 20: se um alvo for morto por uma tempestade arcana, a recarga de Conflux é zerada.<br><br>" +  

                "Conflux, nível 6<br>" + 
                "comando: [ArchmageConflux <br>" + 
                "quando ativado, o dano de feitiço do archmage é aumentado em 10% por (10 +1/nível) segundos, 2 minutos de recarga.<br>" + 
                "nível 12: o dano de feitiço é aumentado em 15%.<br><br>" +

                "Mana Singularity, nível 11.<br>" +
                "comando: [ArchmageMassSingularity <br>" +
                "O Archmage cria um vórtice arcano em colapso em um local alvo a até 2 + (nível/3) tiles. Após 2.5 segundos, a singularidade<br>" + 
                "explode, causando 25-40 + (int/10 + nível/2) de dano de energia a todos os alvos hostis a até 2 tiles de distância do ponto de origem.<br>" + 
                "Alvos atingidos pela explosão também perdem 20+nível de mana. O archmage recupera 4 de mana por inimigo atingido. Ativar esta habilidade custa 50 de mana.<br>" +
                "Esta habilidade tem uma recarga de 60 segundos.<br>" + 
                "Nível 16: todos os inimigos atingidos por esta habilidade ficam paralisados por 4 segundos e perdem 20+nível de vigor.<br><br>" +

                "Timestop, nível 18<br>" +
                "comando:[ArchmageTimestop <br>" +
                "todas as criaturas hostis a até (3+(nível/5)) tiles de distância do conjurador ficam paralisadas por (12+nível/4) segundos.<br>" +
                "Esta habilidade custa 60 de mana e tem 3 minutos de recarga.<br>" +
                "Nível 20: quando a paralisia termina, todos os inimigos atingidos por esta habilidade perdem 40+nível de mana e vigor.<br><br>" +

                "mana vault, nível 2<br>" +
                "quando um inimigo tentar sugar ou drenar mana do archmage, há 2% de chance por nível de que a mana seja imediatamente devolvida ao archmage.<br>" +
                "Nível 5: quando um inimigo tentar sugar ou drenar mana do archmage, ele recebe 15-25 + (nível/2) de dano de energia.<br>"+
                "Nível 13: quando um inimigo tentar sugar ou drenar mana do archmage, ele fica paralisado por 2 segundos.<br><br>"+

                "weave reflection, nível 8<br>"+
                "- o archmage recebe 2.5%/nível de chance de refletir feitiços.<br>"+
                "Nível 17: quando o archmage reflete um feitiço, ele tem 1% de chance / nível de zerar a recarga de Conflux.<br>"+

                "arcane tempest, nível 14<br>"+
                "ao lançar um feitiço em um alvo hostil, há (0.25 * nível)% de chance de desencadear uma Mana Singularity no alvo que custa<br>"+
                " sem mana e não tem recarga.<br>"+
                "-Nível 19: quando arcane tempest é desencadeada, há 1% de chance/nível de que a recarga de arcane storm seja zerada.<br><br>"+

                "Weave Unraveling, nível 20<br>"+
                "ao lançar um feitiço de magia prejudicial em um alvo, há 0.25% de chance por nível de criar distorções no Tecido ao redor dele.<br>"+ 
                "A quantidade de distorções varia entre 6-12, e elas duram de 12 a 22 segundos.<br>"+ 
                "Qualquer um que estiver sobre essas distorções recebe 14-22 + (int/15) de dano de energia por segundo.<br>"+ 
                "</BASEFONT>";  
            }
            else if ( type == AscensionType.Palemaster)
            {
                return
                "<BASEFONT COLOR=#FFFFFF>" +
                "Estas são as habilidades que o Palemaster aprende ao subir de nível:<br><br>" + 
                "Undying hordes, nível 1<br>" + 
                "comando: [PalemasterUndyingHordes<br>" + 
                "O Palemaster invoca uma horda de mortos-vivos sem mente para atacar seus inimigos.<br>" + 
                "Estes atacarão qualquer coisa hostil ao conjurador incansavelmente até serem destruídos ou até se desfazerem em pó.<br>" + 
                "A Horda Imortal tem 3 minutos de recarga entre usos.<br>" +    
                "A horda dura 60 (+3 por nível) segundos e sua composição é baseada no Nível do Palemaster.<br>" +  
                "Nível 1-4: 3-5 esqueletos, 2-3 guerreiros esqueletos<br>" + 
                "Nível 5-8: 3-5 esqueletos, 2-3 guerreiros esqueletos, 1-2 cavaleiros esqueletos<br>" + 
                "Nível 9-12: 3-4 guerreiros esqueletos, 2-3 cavaleiros esqueletos, 1-2 múmias<br>" + 
                "Nível 13-16: 2-3 cavaleiros esqueletos, 2-3 múmias, 1-2 múmias antigas, 1 gigante morto-vivo<br>" + 
                "Nível 17-19: 3-4 gigantes mortos-vivos, 2-3 múmias antigas, 1 dragão esquelético<br>" + 
                "Nível 20: 4-5 gigantes mortos-vivos, 3-4 múmias antigas, 1-2 dragões esqueléticos<br><br>" +          

                "Enervate, nível 6<br>" + 
                "comando: [PalemasterEnervate<br>" + 
                "O palemaster suga a vida de seus inimigos, alimentando-se e enfraquecendo-os.<br>" +  
                "Quando conjurado, este feitiço faz com que todas as criaturas vivas hostis em até 3 + (1 a cada 5 níveis) tiles recebam 40 + (nível /3 ) de dano de frio e percam a mesma quantidade em mana.<br>" +  
                "Para cada criatura atingida, o palemaster recupera 5 pontos de vida e 5 de mana, até um máximo de 50 de cada.<br>" + 
                "As criaturas atingidas também recebem -(5 + nível do palemaster / 4)% em sua força e destreza por 30 segundos.<br>" +
                "Esta habilidade custa 30 de mana para ativar e tem um minuto de recarga.<br>" + 
                "Nível 12: a quantidade máxima recuperada é limitada a 100 para pontos de vida e mana.<br><br>" +         

                "Circle Of Death, nível 11.<br>" +
                "comando: [PalemasterCircleOfDeath.<br>" +
                "O palemaster corrompe o tecido para corroer a vida de seus inimigos.<br>" +
                "Esta habilidade cria um círculo ao redor da criatura alvo, e todas as criaturas vivas a até 2 (+1 a cada 5 níveis) dela recebem 12-18 + (nível /2 ) + (int / 20) de dano de frio por segundo por uma quantidade de segundos igual a 3 + nível / 4.<br>" +
                "Esta habilidade tem 2 minutos de recarga e custa 60 de mana para ativar.<br>" +
                "Nível 16: criaturas presas no círculo perdem vigor a cada segundo igual à metade do dano causado.<br><br>" +            

                "Danse Macabre, nível 18.<br>" +
                "comando: [PalemasterDanseMacabre<br>"+
                "O Palemaster estilhaça o portal entre mundos, trazendo as hordas famintas para a terra dos vivos.<br>" + 
                "Esta habilidade tem 4 minutos de recarga e custa 80 de mana para ativar.<br>" +
                "A cada segundo por nível/3 segundos, um dos seguintes eventos acontece:<br>" +
                "    - 1-2 gigantes mortos-vivos são gerados e atacam os inimigos do palemaster, eles duram 30 segundos.<br>" + 
                "    - 2-3 senhores múmia são gerados e atacam os inimigos do palemaster, eles duram 30 segundos.<br>" +
                "    - todo morto-vivo sob o controle do palemaster que esteja a até 5 tiles de distância do palemaster cura uma quantidade de pontos de vida igual a 25% de seus pontos de vida máximos.<br>" +
                "    - o palemaster cura completamente sua vida.<br>" +
                            
                "no nível 20, estes efeitos são adicionados aos possíveis efeitos que acontecem a cada segundo:<br>" +
                "    - toda criatura viva hostil que esteja a até 3 tiles de distância do palemaster e que esteja com menos de 50% de seus pontos de vida máximos perde 10% de seus pontos de vida.<br>" +
                "    - todas as criaturas vivas a até 4 tiles de distância do palemaster recebem um debuff de -20% em sua força que dura 30 segundos.<br>" +
                "    - 1 dragão esquelético é gerado e ataca os inimigos do palemaster, ele dura 30 segundos.<br><br>" +

                "deathless vigor, nível 2.<br>" +
                "Ao lançar um feitiço ofensivo de necromancia, 1.5% por nível de chance de recuperar 2 + (nível / 2) pontos de vida.<br>" + 
                "Nível 5: 0.25% por nível de chance de ganhar +10% de dano de feitiço por 15 segundos.<br>" +
                "Nível 13: o bônus de dano de feitiço aumenta para +15%, a duração aumenta para 25 e a chance aumenta para 0.45% por nível.<br><br>" +

                "undead graft, nível 8.<br>" +
                "Ao lançar um feitiço ofensivo de necromancia enquanto usa um conjunto completo de armadura de osso, há 0.25% por nível de chance de que todas as criaturas mortas-vivas invocadas amigáveis curem 5% de seus pontos de vida.<br>" +
                "Nível 17: quando undead graft é ativado, criaturas mortas-vivas invocadas amigáveis recuperam 2% de sua saúde máxima por segundo por nível/2 segundos.<br><br>" + 
                        
                "creeping cold, nível 14.<br>" +
                "Ao lançar wither, há 0.25% de chance por nível de que ele seja ativado uma vez adicional.<br>" +
                "Nível 19: quando o efeito creeping cold é ativado, há 0.25% de chance por nível de que enervate seja lançado automaticamente ignorando custos de mana e recarga.<br>" +
                        
                "herald of hereafter, nível 20.<br>" +
                "Ao lançar um feitiço prejudicial de necromancia em um alvo, há 0.25% de chance por nível de criar distorções no Tecido ao redor dele. A quantidade de distorções varia entre 6-12, e elas duram de 12 a 22 segundos.<br>" +
                "Qualquer um que estiver sobre essas distorções recebe 14-22 + (int/15) de dano de veneno por segundo.<br>"+ 

                "</BASEFONT>"; 
            }
            else if (type == AscensionType.Crusader)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "Smite, nível 1.<br>" +
                "comando: [CrusaderSmite<br>" +
                "O crusader canaliza seu valor e invoca uma explosão de chama purificadora.<br>" + 
                "Este efeito causa 20-32 +( nível / 2 + for/15) de dano a todas as criaturas malignas que estão a até 2 (+1 a cada 5 níveis) tiles de distância do crusader.<br>" +
                "Smite custa 20 de mana e 20 de vigor para ativar, e tem 1 minuto de recarga.<br>" +
                "- Nível 10: criaturas hostis que tenham karma de -10k ou inferior ficam paralisadas por 4(+1 a cada 6 níveis) segundos após serem atingidas pelo smite.<br>" +
                "- Nível 15: criaturas amigáveis com karma positivo pegas na área do smite são curadas em 20 + nível/2 pontos de vida.<br>" +
                "- Nível 20: criaturas malignas hostis que tenham karma de -10k ou inferior danificadas pelo smite são pegas em chamas abrasadoras. Elas recebem 10-20 + for/15 de dano de fogo a cada 2 segundos por 12 segundos.<br><br>" +

                "Charge, nível 6.<br>" +
                "comando: [CrusaderCharge<br>" +
                "Quando usado, o personagem escolhe um tile livre que esteja a pelo menos 2 tiles de distância dele e é movido para aquele tile.<br>" +
                "O alcance máximo é igual a 3 + nível/5. Ao chegar, toda criatura hostil adjacente àquele tile tem 1% por nível de chance de ser paralisada por 3 segundos.<br>" +  
                "Tem uma recarga de 9 segundos, menos um segundo a cada 3 níveis de crusader.<br>" +
                "- Nível 12: ao chegar, todas as criaturas hostis adjacentes ao quadrado de destino recebem 30-45 + ((for / 15) + nível / 3) de dano físico.<br><br>" +

                "Aura of Hope, nível 11.<br>" +
                "comando: [CrusaderAuraOfHope<br>" +
                "Quando usado, o crusader e todas as criaturas amigáveis a até 4 tiles de distância do crusader recebem os seguintes benefícios:<br>" +
                "- eles restauram 12 + (nível de crusader / 2) pontos de vida a cada 4 segundos por 30 segundos.<br>" +
                "- eles restauram 6 + (nível de crusader / 2) de mana a cada 4 segundos por 30 segundos.<br>" +
                "- eles restauram 6 + (nível de crusader / 2) de vigor a cada 4 segundos por 30 segundos.<br>" +
                "Esta habilidade tem 3 minutos de recarga, e custa 40 de mana e 40 de vigor para ativar.<br>" +
                "- Nível 16: criaturas afetadas pela aura da esperança são curadas completamente uma vez que o buff expira.<br><br>" +

                "Heavenly Gate, nível 18.<br>" +
                "comando: [CrusaderHeavensGate<br>" +
                "O crusader abre os portões celestiais e invoca um companheiro celestial para ajudá-lo em sua guerra contra todo o mal.<br>" +
                "A criatura é um poderoso Archon que requer 2 slots de controle e permanece ao lado do crusader até ser derrotado.<br>" +
                "Ativar esta habilidade custa 60 de mana e 60 de vigor. Tem 5 minutos de recarga.<br>" +
                "- Nível 20: Há 1% de chance por nível de que dois archons sejam invocados em vez de um.<br><br>" + 
                        
                "Divine Grace, nível 2.<br>" +
                "o crusader tem 4% de chance por nível de chance de curar automaticamente de venenos menores, regulares e maiores.<br>" + 
                "- Nível 5: a chance também se aplica a venenos mortais.<br>" +
                "- Nível 13: a chance também se aplica a venenos letais.<br><br>" +

                "Holy Fervor, nível 8.<br>" +
                "Quando o crusader mata um demônio, ele tem 1.5% por nível de chance de desencadear uma explosão de fogo sagrado em área, causando 20-32 + (nível/2 + for/15) de dano a todas as criaturas malignas hostis que estão a até 2 (+1 a cada 5 níveis) tiles de distância do crusader.<br>" +
                "- Nível 17: quando Holy Fervor é ativado, a Recarga de Smite é zerada.<br>" +
                        
                "Inquisitorial Strikes, nível 14.<br>" +
                "os golpes de arma corpo a corpo do crusader causam + (nível/2)% de dano contra demônios.<br>" +
                "- Nível 19: os golpes de arma corpo a corpo do crusader causam + (nível/4)% de dano contra criaturas malignas.<br>" +
                        
                "Divine Judgement, nível 20.<br>" +
                "Quando o crusader mata uma criatura maligna poderosa, há 2.5% de chance de que uma criatura poderosa chamada Luminar seja invocada para ajudar o Crusader em sua guerra contra o mal.<br>" +
                "A criatura atacará qualquer criatura oponente próxima com grande ferocidade e permanecerá no plano material por um minuto.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Assassin)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "Noxious Cloud, nível 1.<br>" +
                "comando: [AssassinNoxiousClou<br>" +
                "O assassino cria uma nuvem de vapores fétidos no local alvo que envenena todos os inimigos pegos em sua área.<br>" +
                "A nuvem inflige veneno maior em todos os inimigos afetados. Esta habilidade custa 30 de mana e 30 de vigor para ativar.<br>" + 
                "Esta habilidade tem 1 minuto de recarga. A área da nuvem é igual a 1 + 1 a cada 6 níveis.<br>" +
                "- Nível 10: A nuvem inflige Veneno mortal em todos os inimigos afetados.<br>" +
                "- Nível 15: A nuvem inflige Veneno letal em todos os inimigos afetados.<br>" +
                "- Nível 20: todos os inimigos afetados perdem 20 de resistência a veneno por 30 segundos.<br><br>" +

                "Crippling poison, nível 6.<br>" +
                "comando: [AssassinCripplingPoison<br>" +
                "O assassino arremessa um frasco de veneno incapacitante em um alvo, infligindo veneno mortal e impedindo que ele ande por nível / 2 segundos.<br>" +
                "Esta habilidade custa 40 de mana e 40 de vigor para ativar e tem 1 minuto de recarga.<br>" +
                "- Nível 12: O veneno incapacitante é Letal e afeta todas as criaturas hostis a até 2 tiles de distância do alvo original.<br><br>" + 

                "Toxic surge, nível 11.<br>" +
                "comando: [AssassinToxicSurge<br>" +
                "Pelos próximos 30 segundos, o assassino causa +10% de bônus de dano com golpes de arma contra criaturas envenenadas.<br>" + 
                "Esta habilidade custa 40 de mana e 40 de vigor para ativar e tem 2 minutos de recarga.<br>" +
                "- Nível 16: Quando ativado, há 1% de chance por nível de zerar a recarga de Noxious Cloud.<br>" + 

                "Cleansing Annihilation, nível 18.<br>" +
                "comando: [AssassinCleansingAnnihilation<br>" +
                "O assassino inflige veneno letal no alvo, então consome imediatamente todos os seus tiques.<br>" + 
                "Se o alvo sobreviver, todas as criaturas ao redor dele a até 3 tiles de distância são infligidas com veneno letal.<br>" + 
                "Ativar esta habilidade custa 50 de mana e 50 de vigor e tem 4 minutos de recarga.<br>" +
                "- Nível 20: ao ativar esta habilidade, há 1% de chance por nível de que a recarga de toxic surge seja zerada.<br><br>" +

                "Virulent Strikes, nível 2.<br>" +
                "Ao aplicar um veneno em um alvo com um golpe, o assassino tem 1% por nível de chance de resolver imediatamente 1 tique de veneno.<br>" +
                "- Nível 5: ao fazer um ataque de arma contra uma criatura envenenada, o assassino tem 0.25% de chance por nível de resolver imediatamente um tique de veneno.<br>" +
                "- Nível 13: ao fazer um ataque de arma contra uma criatura envenenada, o assassino tem 0.12% de chance por nível de resolver imediatamente um tique de veneno adicional.<br><br>" +

                "dangerous habits, nível 8.<br>" +
                "aumenta o dano dos tiques de veneno infligidos pelo assassino em 9%.<br>" +
                "- Nível 17: aumenta o dano dos tiques de veneno infligidos pelo assassino em 18%.<br><br>" +

                "Deadly Strikes, nível 14.<br>" +
                "O assassino causa 9% mais dano em ataques a criaturas envenenadas.<br>" +
                "- Nível 19: ataques contra criaturas envenenadas ignoram 25% da resistência a veneno do alvo.<br><br>" +

                "Terminal, nível 20.<br>" +
                "Ao matar uma criatura envenenada, o assassino tem 0.25% de chance por nível de infligir veneno letal em todas as criaturas hostis que estão a até 2 tiles de distância dela.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Blackguard)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "Dark Succor, nível 1.<br>" +
                "comando: [BlackguardDarkSuccor<br>" +
                "O blackguard entra em um transe profano por (30 +1 por nível) segundos.<br>" +
                "Dark Succor custa 20 de mana para ativar. Enquanto ativo, o Blackguard tem 15 + nível/2 de força.<br>" + 
                "Quando Dark Succor termina, o blackguard perde metade de sua mana restante. Esta habilidade tem 90 segundos de recarga.<br>" +
                "- Nível 10: Enquanto no transe e abaixo de 50% de seus pontos de vida máximos, o blackguard causa + 0.75% de dano por nível contra criaturas de karma positivo.<br>" +
                "- Nível 15: Enquanto no transe e abaixo de 50% de sua mana máxima, sempre que o blackguard mata uma criatura com um golpe de arma, o blackguard recupera 2 + nível/2 de mana.<br>" + 
                "- Nível 20: Enquanto no transe e abaixo de 50% de seus pontos de vida máximos, sempre que o blackguard mata uma criatura com um golpe de arma, o blackguard se cura em 6 + nível/2 pontos de vida.<br><br>" +

                "Death's Advance, nível 6.<br>" +
                "comando: [BlackguardDeathsAdvance<br>" +
                "Quando usado, o blackguard escolhe um tile livre que esteja a pelo menos 2 tiles de distância dele e é movido para aquele tile. O alcance máximo é igual a 3 + nível/5. Ao chegar, toda criatura hostil adjacente àquele tile tem 1% por nível de chance de ser repelida por 2 + 1 tile a cada 5 níveis do blackguard.<br>" +  
                "Tem uma recarga de 9 segundos, menos um segundo a cada 3 níveis de Blackguard.<br>" + 
                "- Nível 12: ao chegar, todas as criaturas hostis adjacentes ao quadrado de destino recebem 30-45 + ((for / 15) + nível / 3) de dano físico.<br><br>" +

                "Chains of Ice, nível 11.<br>" +
                "comando: [BlackguardChainsOfIce<br>" +
                "O blackguard lança correntes de gelo ao seu redor, congelando todas as criaturas hostis por 3 + 1 a cada 4 níveis segundos. Criaturas pegas pelas correntes recebem 30-43 + (for/15) de dano de frio.<br>" +
                "custa 30 de mana para ativar e tem 90 segundos de recarga.<br>" +
                "- Nível 16: Quando a paralisia termina, as correntes explodem causando 30-43 + (for/15) de dano de frio adicional a cada criatura adjacente a uma criatura afetada.<br><br>" +

                "Frostwyrm's Fury, nível 18.<br>" +
                "comando: [BlackguardsFrostwyrmsFury<br>" +
                "O blackguard abre os portões do submundo e invoca um dragão morto-vivo ligado à sua vontade.<br>" +
                "A criatura é um poderoso dracolich que requer 4 slots de controle e permanece ao lado do blackguard até ser derrotado.<br>" + 
                "Ativar esta habilidade custa 60 de mana e 60 de vigor. Tem 5 minutos de recarga.<br>" +
                "- Nível 20: há 1% de chance por nível de que dois frostwyrms sejam invocados em vez de um.<br><br>" + 

                "Frozen Heart, nível 2.<br>" +
                "Aumenta o dano de frio causado pelos golpes de arma do blackguard em 6%.<br>" +
                "- Nível 5: Aumenta o dano de frio causado pelos golpes de arma do blackguard em 12%.<br>" +
                "- Nível 13: Aumenta o dano de frio causado pelos golpes de arma do blackguard em 18%.<br><br>" +

                "Morbidity, nível 8.<br>" +
                "Quando o blackguard mata uma criatura de karma positivo, ele tem 0.25% de chance por nível de aterrorizar a maioria dos inimigos próximos a até 2 tiles de distância, fazendo-os fugir por 3 segundos.<br>" + 
                "- Nível 17: Inimigos aterrorizados fogem por 6 segundos em vez disso.<br><br>" +

                "Merciless Strikes, nível 14.<br>" +
                "Os ataques de arma do blackguard têm 1% de chance por nível de afetar a resistência mais fraca do alvo.<br>" +
                "- Nível 19: Os golpes impiedosos do blackguard têm 0.25% de chance por nível de paralisar o alvo por 3 segundos.<br><br>" +

                "Soul Reaper, nível 20.<br>" +
                "Ao matar uma criatura hostil, o blackguard tem 0.25% de chance por nível de sugar nível + for /25 de vida de todas as criaturas hostis que estão a até 2 tiles de distância dela.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Skald)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "War Chant, nível 1<br>" +
                "comando: [SkaldWarChant<br>" +
                "O Skald canaliza uma canção poderosa que fortalece seus aliados.<br>" +
                "Por 10 (+1 por nível) segundos, o skald e todas as criaturas amigáveis a até 4 tiles de distância recebem (5 + 1 por nível) de força e destreza, e um bônus de (5 +1 por nível) em táticas e resistência mágica.<br>" +
                "Esta habilidade custa 20 de mana para ativar e tem 90 segundos de recarga.<br>" +
                "- Nível 10: O skald e seus aliados recebem um bônus de 0.5% por nível de dano com armas enquanto o war chant está ativo.<br>" +
                "- Nível 15: Quando o skald ou seus aliados derrotam um inimigo enquanto o war chant está ativo, eles recuperam 5 de vida e 3 de vigor.<br>" +
                "- Nível 20: Quando o War Chant termina no Skald, há 2% de chance por nível de ele ser lançado novamente ignorando sua recarga e custo de mana.<br><br>" +

                "Saga of Valor, nível 6<br>" +
                "comando: [SkaldSagaOfValor<br>" +
                "O skald e todos os seus aliados a até 4 tiles de distância recebem +(nível/2)% de bônus de chance de acerto por 15 +1 por nível segundos.<br>" + 
                "Esta habilidade custa 30 de mana para ativar e tem 90 segundos de recarga.<br>" +
                "- Nível 12: O skald e todos os seus aliados a até 4 tiles de distância recebem um aumento de +(nível/2)% de chance de defesa durante a duração da saga.<br><br>" +

                "Song of Thunder, nível 11<br>" +
                "comando: [SkaldSongOfThunder<br>" +
                "O skald invoca relâmpagos para atingir seus inimigos. Por 15 + 1 por nível segundos, um raio atinge um alvo hostil aleatório próximo a cada 3 segundos, causando 30-45 (+ fama do skald / 1000) de dano de energia, até um alcance máximo de 4 tiles.<br>" + 
                "Esta habilidade custa 40 de mana para ativar e tem 180 segundos de recarga.<br>" +
                "- Nível 16: A cada 3 segundos após o primeiro raio, um raio adicional atinge um alvo hostil aleatório diferente próximo.<br><br>" + 

                "Dirge Of The Fallen, nível 18<br>" +
                "comando: [SkaldDirgeOfTheFallen<br>" +
                "O skald canta sobre o valor de heróis antigos e os traz para a batalha. Quando esta canção é tocada, o skald invoca 3 + (nível/4) guerreiros antigos da lenda que atacarão os inimigos do skald até serem derrotados.<br>" +
                "Os guerreiros antigos duram no máximo 90 segundos. Esta habilidade custa 50 de mana para ativar e tem 5 minutos de recarga.<br>" +
                "- Nível 20: Ao usar Dirge of The Fallen, há 1% de chance por nível de Song of Thunder ser ativado automaticamente.<br><br>" +

                "Battlefield Rhythm, nível 2<br>" +
                "O skald ganha um bônus de 3% de dano com armas contra criaturas que estão em discórdia, provocadas ou pacificadas.<br>" +
                "- Nível 5: O skald ganha um bônus de 9% de dano com armas contra criaturas que estão em discórdia, provocadas ou pacificadas.<br>" +
                "- Nível 13: O skald ganha um bônus de 18% de dano com armas contra criaturas que estão em discórdia, provocadas ou pacificadas.<br><br>" +

                "Cutting Words, nível 8<br>" +
                "As tentativas de discórdia do skald suprimem 4% adicional dos atributos e habilidades do alvo.<br>" +
                "- Nível 17: As tentativas de discórdia do skald suprimem 8% adicional dos atributos e habilidades do alvo.<br><br>" +

                "Ressonance, nível 14.<br>" +
                "Sua proeza musical é considerada 15% maior para feitiços de canção.<br>" +
                "- Nível 19: Sua proeza musical é considerada 25% maior para feitiços de canção.<br><br>" +

                "Saga of Steel, nível 20<br>" +
                "Ao matar uma criatura hostil com uma arma, o Skald tem 0.25% de chance por nível de ativar instantaneamente um feitiço de canção Foe Requiem no inimigo hostil mais próximo.<br>"+
                "</BASEFONT>";
            }
            else if (type == AscensionType.Reaver)
            {
                return
                "<BASEFONT COLOR=#ffffff>"+
                "Gorge, nível 1.<br>" +
                "comando: [ReaverGorge<br>" +
                "O Reaver lança uma maldição ao redor de uma área alvo, espalhando sangue espesso e vicioso sobre ela (raio 2 + 1 / 4 níveis).<br>" +
                "Criaturas hostis na área recebem -1.25 de resistência física por nível do reaver. A área dura 10 + 1/nível segundos.<br>" + 
                "Esta habilidade custa 15 de vigor e 15 de mana para ativar e tem 3 minutos de recarga.<br>" +
                "- Nível 10: Criaturas pegas dentro da área do gorge ficam lentas e não podem correr.<br>" + 
                "- Nível 15: Criaturas pegas dentro da área do gorge recebem -0.66 de penalidade por nível do reaver em todas as resistências elementais.<br>" + 
                "- Nível 20: Quando uma criatura morre dentro da área do gorge, há 1.5% de chance por nível do reaver de que seu cadáver exploda, causando 40-66 + (for do reaver / 15 + táticas do reaver/12) de dano físico a todas as criaturas hostis que estão a até 2 tiles de distância dele.<br><br>" + 

                "Exsanguinate, nível 6.<br>" +
                "comando: [ReaverExsanguinate <br>" +
                "O Reaver realiza um ataque de redemoinho que faz todos os alvos adjacentes sangrarem. O ataque causa 18-26 + for/15 de dano físico, e o sangramento causa 26-32 de dano físico a cada 3 segundos por 13 + nível segundos.<br>" +
                "Esta habilidade custa 20 de mana e 20 de vigor para ativar, e tem 1 minuto de recarga.<br>" +
                "- Nível 12: o reaver recupera 9 de vida para cada inimigo afetado pelo Exsanguinate.<br><br>" +

                "Bloodstorm, nível 11. <br>" +
                "comando: [ReaverBloodstorm <br>" +
                "O reaver canaliza seu desdém e invoca uma explosão de sangue. Este efeito causa 20-32 + (nível/2 + for/15) de dano a todas as criaturas que podem sangrar e estão a até 3 (+1 a cada 5 níveis) tiles de distância do reaver.<br>" +
                "Exsanguinate custa 25 de mana e 25 de vigor para ativar, e tem 2 minutos de recarga.<br>" +
                "- Nível 16: Quando usado, há 0.5% de chance por nível de que a recarga de Exsanguinate seja zerada.<br><br>" +

                "Absolute Tyranny, nível 18. <br>" +
                "comando: [ReaverAbsoluteTyranny <br>" +
                "Pelos próximos 30 segundos, o Reaver causa 18% de dano físico extra com machados, e quando mata um inimigo com um golpe de machado, recupera 10% de seus pontos de vida e vigor.<br>" + 
                "Absolute tyranny custa 30 de mana e 30 de vigor para ativar, e tem 3 minutos de recarga.<br>" +   
                "- Nível 20: Quando um Reaver mata um inimigo enquanto sob o efeito de Absolute Tyranny, há 1% de chance por nível de que seu cadáver exploda, causando 30-56 + (for/15 + táticas/12) de dano físico a todas as criaturas hostis que estão a até 2 tiles de distância dele.<br>" +

                "Leech, nível 2. <br>" +
                "Quando o reaver faz um golpe de machado contra um inimigo, ele tem 0.25% de chance por nível de drenar 0.10%/nível de pontos de vida do alvo.<br>" +
                "- Nível 5: Quando o reaver faz um golpe de machado contra um inimigo, ele tem 0.25% de chance por nível de drenar 0.12%/nível de mana do alvo.<br>" +
                "- Nível 13: quando o reaver faz um golpe de machado contra um inimigo que tem 25% ou menos de pontos de vida restantes, ele tem 0.5%/nível de chance de drenar 1%/nível adicional de pontos de vida do alvo.<br><br>" +

                "Ruthless, nível 8. <br>" +
                "O reaver causa 9% de dano extra com machados.<br>" +
                "- Nível 17: O reaver causa 18% de dano extra com machados.<br><br>" +
                    
                "Flaying Strikes, nível 14. <br>" +
                "Os ataques de arma do Reaver com machados têm 1% de chance por nível de afetar a resistência mais fraca do alvo.<br>" +
                "- Nível 19: Os golpes impiedosos do Reaver têm 0.25% de chance por nível de zerar a recarga de Gorge.<br><br>" +

                "Deep Cuts, nível 20.<br>" +
                "Ao matar uma criatura hostil, o reaver tem 0.25% de chance por nível de aplicar um sangramento pesado em todas as criaturas hostis adjacentes a ela.<br>" + 
                "Essas criaturas ficam aleijadas (não podem correr) e sofrem 52-72 + for/10 de dano a cada 3 segundos por 12 a 21 segundos. Este efeito não acumula.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Kensai)
            {
                return 
                "<BASEFONT COLOR=#ffffff>"+
                "Battle Meditation, nível 1<br>" +
                "comando: [KensaiBattleMeditation<br>" +
                "O Kensai entra em um poderoso transe de batalha. Pelos próximos 20 + 1/nível segundos, o Kensai causa +(10 + nível/2) % de dano com espadas e tem +(5 + nível/2)% de aumento de velocidade de ataque.<br>" + 
                "Isso custa 20 de vigor para ativar e tem 2 minutos de recarga.<br>" +
                "- Nível 10: Enquanto sob os efeitos da meditação de batalha, o Kensai tem +(10+nível/2) de aumento de chance de defesa.<br>" +
                "- Nível 15: Quando o Kensai atinge um inimigo enquanto sob os efeitos da meditação de batalha, há 1%/nível de chance de que a recarga de Culling Strike seja zerada.<br>" +
                "- Nível 20: Quando o Kensai mata um inimigo enquanto sob os efeitos da meditação de batalha, a recarga de tempest é reduzida em 5 segundos.<br><br>" +

                "Kai, nível 6<br>" +
                "comando: [KensaiKai<br>" +
                "Quando usado, o personagem escolhe um tile livre que esteja a pelo menos 2 tiles de distância dele e é movido para aquele tile. O alcance máximo é igual a 3 + nível/4. Ao aterrissar, o personagem causa 20-35 + ( (Des / 15) + nível / 3 ) de dano em todos os alvos adjacentes àquele tile. Tem uma recarga de 9 segundos, menos um segundo a cada 3 níveis de Kensai.<br>" + 
                "Esta habilidade custa 30 de vigor para ativar.<br>" +
                "- Nível 12: ao aterrissar, o kensai tem 1%/nível de chance de realizar um culling strike em todos os alvos adjacentes se o kensai estiver usando uma espada.<br><br>" +

                "Culling Strike, nível 11<br>" +
                "comando: [KensaiCullingStrike<br>" +
                "Pelos próximos 30 segundos, sempre que o Kensai faz um ataque com uma espada contra um alvo que tem menos de 10% de seus pontos de vida totais, o Kensai tem 2%/nível de chance de causar + 80% de dano com esse golpe.<br>" +
                "Esta habilidade custa 50 de vigor para ativar e tem 3 minutos de recarga.<br>" +
                "- Nível 16: O bônus de dano do culling strike pode ser ativado contra oponentes que têm menos de 15% de seus pontos de vida totais.<br><br>" +

                "Tempest, nível 18<br>" +
                "comando: [KensaiTempest<br>" +
                "O Kensai se torna uma tempestade de espadas. Quando esta habilidade é ativada, o Kensai atacará todos os inimigos a até 6 tiles de distância, causando 70-85 + des/15 de dano físico.<br>" +
                "Esta habilidade custa 75 de vigor para ativar e tem 1 minuto de recarga.<br>" +
                "- Nível 20: Quando Tempest mata um inimigo, há 0.5%/nível de chance de que ele seja ativado novamente imediatamente sem exigir o custo de vigor.<br><br>" + 

                "Practiced Perfection, nível 2<br>" +
                "O Kensai causa 6% de dano aumentado com espadas.<br>" + 
                "- Nível 5: O Kensai causa 12% de dano aumentado com espadas.<br>" + 
                "- Nível 13: O Kensai causa 18% de dano aumentado com espadas.<br><br>" + 

                "Singular Focus, nível 8<br>" +
                "Quando o Kensai mata um inimigo que estava com vida máxima com um ataque de espada, ele ganha 9% de dano extra com espadas por 30 segundos. Este efeito não acumula.<br>" +
                "- Nível 17: Quando o Kensai mata um inimigo que estava com vida máxima com um ataque de espada, ele ganha 18% de dano extra com espadas por 30 segundos. Este efeito não acumula.<br><br>" +

                "Iaijutsu, nível 14<br>" +
                "Os ataques de arma do Kensai com espadas têm 1% de chance por nível de afetar a resistência mais fraca do alvo.<br>" +
                "- Nível 19: Os ataques de Iaijutsu do Kensai têm 0.25% de chance por nível de serem ativados uma vez adicional.<br><br>" +

                "Final Cut, nível 20<br>" +
                "Quando o Kensai mata um inimigo com vida máxima com um ataque de espada, ele tem 0.25% de chance por nível de ativar uma Tempest que não custa vigor.<br>" +
                "</BASEFONT>";
            }
            else if (type == AscensionType.Hierophant)
            {
                return
                "<BASEFONT COLOR='#FFFFFF'>"+
                "Divine Wrath, nível 1<br>" +
                "comando: [HierophantDivineWrath<br>" +
                "O Hierophant invoca vingança divina. O Hierophant seleciona um local alvo, e fogo divino cai sobre ele, causando 30-48 + (karma/1000) de dano de fogo em todos os alvos a até 2 tiles de distância desse local.<br>" +
                "Criaturas com karma negativo recebem + 15% de dano desta habilidade.<br>" +
                "Esta habilidade custa 35 de mana, e tem 1 minuto de recarga.<br>" +
                "- Nível 10: Criaturas com Karma negativo recebem + 25% de dano desta habilidade.<br>" +
                "- Nível 15: Criaturas com Karma negativo atingidas por esta habilidade ficam paralisadas por 4 segundos e recebem 15-24 de dano de fogo a cada 2 segundos por 12 + nível/3 segundos.<br>" +
                "- Nível 20: Quando esta habilidade é lançada, há 20% de chance de que ela seja lançada novamente imediatamente.<br><br>" + 
                        
                "Exalted Presence, nível 6<br>" +
                "comando: [HierophantExaltedPresence<br>" +
                "O Hierophant invoca o poder de seu deus para humilhar e inspirar temor em todos os inimigos. Eles são forçados a se mover nível/3 tiles para longe do Hierophant e param de atacar o hierophant imediatamente.<br>" + 
                "Esta habilidade custa 45 de mana para ativar, e tem 2 minutos de recarga.<br>" +
                "- Nível 12: Criaturas malignas afetadas por Exalted Presence ficam paralisadas por 8 segundos.<br><br>" + 
                            
                "Consecrated Ground, nível 11<br>" +
                "comando: [HierophantConsecratedGround<br>" +
                "O Hierophant invoca luz sagrada para purificar até 4 tiles ao redor. Criaturas de Karma positivo sobre ele recuperam 12 + nível/3 de pontos de vida e 5 + nível/3 de mana a cada 2 segundos por nível/3 segundos.<br>" +
                "Criaturas de karma negativo sobre o chão consagrado recebem 22-32 + nível/3 de dano de fogo a cada 2 segundos por nível/3 segundos.<br>" +
                "Esta habilidade custa 55 de mana para ativar, e tem 3 minutos de recarga.<br>" +
                "- Nível 16: Quando o Hierophant lança Heal, Greater Heal ou touch of life, o alvo recupera 25% mais pontos de vida.<br><br>" +
                        
                "Divine Power, nível 18<br>" +
                "comando: [HierophantDivinePower<br>" +
                "O Hierophant invoca o poder de seu deus para fortalecê-lo. Ele recebe +20 de for e des, +15 de táticas e espiritualismo, regenera 8 pontos de vida por segundo e causa + 20% de dano com armas de impacto.<br>" + 
                "Este feitiço custa 55 de mana para ativar e tem 3 minutos de recarga. Dura 30+nível segundos.<br>" +
                "- Nível 20: Quando o Hierophant derrota um inimigo com karma negativo em combate, há 20% de chance de que a recarga de Divine Wrath seja zerada.<br><br>" + 
                            
                "Blessed Might, nível 2<br>" +
                "O Hierophant causa 5% mais dano com armas de impacto.<br>" +
                "- Nível 5: O Hierophant causa 10% mais dano com armas de impacto.<br>" +
                "- Nível 13: O Hierophant causa 15% mais dano com armas de impacto.<br><br>" +
                        
                "Divine Resilience, nível 8<br>" +
                "O Hierophant recebe 4% menos dano de todas as fontes.<br>" + 
                "- Nível 17: O Hierophant recebe 8% menos dano de todas as fontes.<br><br>" + 
                        
                "Death Ward, nível 14<br>" +
                "Quando o Hierophant recebe dano que o mataria, ele tem 1% de chance por nível de ignorar esse dano.<br>" + 
                "- Nível 19: Quando Death Ward é ativado, o Hierophant recupera 33% de seus pontos de vida.<br><br>" + 
                        
                "Divine Absolution, nível 20<br>" +
                "Quando o Hierophant recebe dano de uma criatura maligna, ele tem 0.25%/nível de chance de ativar consecrated ground sem pagar seu custo de mana.<br>" + 
                "</BASEFONT>";
            }
            else if (type == AscensionType.ArcaneArcher)
            {
                return
                "<BASEFONT COLOR='#FFFFFF'>"+
                "Imbue Arrow, nível 1<br>"+
                "comando: [ArcaneArcherImbueArrows<br>"+
                "Pelos próximos 30 (+1/nível) segundos, os ataques à distância do arcane archer têm 2%/nível de chance de afetar a pior resistência do alvo.<br>"+ 
                "Esta habilidade custa 40 de mana para ativar e tem 2 minutos de recarga.<br>"+
                "- Nível 10: Quando uma habilidade desencadeada de um ataque à distância ocorreria, há 1%/nível de chance de que ela seja desencadeada duas vezes.<br>"+
                "- Nível 15: Habilidades desencadeadas de ataques à distância causam 25% mais dano enquanto Imbue Arrows está ativo.<br>"+
                "- Nível 20: Quando imbue arrows termina, há 1%/nível de chance de que a recarga de barrage seja zerada.<br><br>"+

                "Charged Arrows, nível 6<br>"+
                "comando: [ArcaneArcherChargedArrows<br>"+
                "O arcane archer libera uma flecha poderosa que danifica tudo em seu caminho por 45-75 + (int/15 + des/15) de dano de um elemento aleatório.<br>"+ 
                "Esta habilidade custa 50 de mana para ativar e tem 1 minuto de recarga. Tem alcance de 6 tiles.<br>"+
                "- Nível 12: quando charged arrows é lançado, há 0.5%/nível de chance de zerar a recarga de Arcane Volley.<br><br>"+

                "Arcane Volley, nível 11<br>"+
                "comando: [ArcaneArcherArcaneVolley<br>"+
                "O arcane archer mira em um local e dispara rapidamente múltiplas flechas nele, atingindo todos os inimigos a até 4 tiles por 55-75 + inscrição/10 de dano de um elemento aleatório.<br>"+ 
                "Esta habilidade custa 60 de mana para ativar e tem 30 segundos de recarga.<br>"+
                "- Nível 16: há 1%/nível de chance de que Arcane volley seja ativado duas vezes quando lançado.<br><br>"+

                "Barrage, nível 18<br>"+
                "comando: [ArcaneArcherBarrage<br>"+
                "O arcane archer dispara uma torrente de flechas em direção ao alvo, atacando nível/4 vezes em um instante. Esta habilidade custa 70 de mana para ativar e tem 3 minutos de recarga.<br>"+
                "- Nível 20: há 0.25%/nível de chance de barrage ser disparado duas vezes quando lançado.<br><br>"+

                "Arcane Precision, nível 2<br>"+
                "O arcane archer causa 6% mais dano com ataques à distância.<br>"+
                "- Nível 5: O arcane archer causa 12% mais dano com ataques à distância.<br>"+
                "- Nível 13: O arcane archer causa 18% mais dano com ataques à distância.<br><br>"+

                "Mystical Ricochet, nível 8<br>"+
                "Ao disparar um ataque à distância, há 0.25%/nível de chance de que o inimigo mais próximo também seja danificado por ele.<br>"+
                "- Nível 17: Ricochet pode ser ativado em até dois inimigos próximos.<br><br>"+

                "Arcane Feedback, nível 14<br>"+
                "Ao matar um oponente com um ataque à distância, o arcane archer recupera inscrição/25 de mana.<br>"+
                "- Nível 19: Ao matar um oponente com um ataque à distância, o arcane archer recupera foco/25 de vigor ou foco/25 de vida.<br><br>"+

                "Arcane Momentum, nível 20<br>"+
                "Ao matar um oponente com um ataque à distância, o arcane archer ganha inscrição/8 de absorção de feitiço por 30 segundos.<br>"+ 
                "</BASEFONT>";
            }
            else
            {
                return "EM DESENVOLVIMENTO.";                
            }
        }

    }
}
