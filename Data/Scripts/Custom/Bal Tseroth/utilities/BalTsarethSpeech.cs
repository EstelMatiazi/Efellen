using Server;
using System.Collections;
using System;

namespace Server.Custom.BalTsareth
{
    public static class BalTsarethSpeech
    {
        private static readonly string[] Lines = new string[]
		{
			"Bal Tsareth te vê… e ele tem fome!",
			"Corre {0}, por favor corre! Não consigo parar isto!",
			"O vento arcano se levanta, sinto-o em meus ossos!",
			"A própria biblioteca grita por teu fim, {0}!",
			"Tu lutas contra a vontade de um mago imortal!",
			"Deuses me ajudem… a voz dela não sai do meu crânio…",
			"Minhas mãos não são minhas! Elas atacam por sua própria vontade perversa!",
			"Vejo os olhos dela quando pisco… queimando… observando…",
			"Por favor, {0}, me derruba antes que eu me torne nada além do fantoche dela!",
			"Sinto meus pensamentos sendo arrancados, um por um…",
			"Faça parar… a magia rasga dentro da minha cabeça!",
			"Eu me lembro de quem eu era… preciso… preciso…",
			"Ele está dentro de mim… sussurrando… rindo…",
			"Minha vontade está desmoronando como pergaminho velho…",
			"Eu te imploro, {0}, acaba com este tormento!",
			"Sinto-me escorregando… não deixe ele tomar o que resta de mim!",
			"Meus pensamentos se desfazem como pergaminhos podres… não consigo segurá-los!",
			"Ele está me apagando… palavra por palavra amaldiçoada!",
			"Por favor, {0}, não quero ser um fantoche!",
			"Ouço ele reescrevendo minha mente… ó Deuses, ouço a pena!",
			"Minhas memórias queimam como se postas em chamas!",
			"Estou me tornando o feitiço dela… nada mais!",
			"Ataca-me, {0}! Melhor a morte que a servidão!",
			"Estou preso atrás dos meus próprios olhos!",
			"Meu corpo se move, mas minha alma grita!",
			"Sinto ele rastejando pelos meus pensamentos como verme!",
			"Meu nome… não consigo lembrar meu próprio nome!",
			"Ele está esculpindo os sigilos dela em minha mente!",
			"Eu te imploro, liberta-me desta tumba viva!",
			"Sinto minha vontade virando cinzas!",
			"Ele manipula minha carne! Faça parar, {0}, faça parar!",
			"Meus gritos estão presos dentro do meu crânio!",
			"Não deixe eu me tornar apenas a sombra dela!",
			"Estou desaparecendo… estou desaparecendo…",
			"Ajuda-me, {0}, antes que eu esteja perdido para sempre!",
			"Sou apenas uma página no grande tomo de Bal Tsareth!",
			"Teu destino já está escrito, {0}!",
			"A vontade dela é minha vontade — tu não podes rompê-la!",
			"Luta se precisas, o fim não muda!",
			"Não há medo aqui, {0}, apenas servidão!",
			"Bal Tsareth desfez minha fraqueza!",
			"Minha carne é tinta, e a magia dela a pena!",
			"Tu não és nada além de uma nota de rodapé no plano dela!",
			"Fui aperfeiçoado pelo domínio dela!",
			"Alegra-te, {0}, pois serás registrado em suas crônicas!",
			"Minha mente está em silêncio, e nesse silêncio só ouço ele!",
			"Eu sirvo contente, livre da dúvida e da fragilidade!",
			"Tua morte adornará a biblioteca dela!",
			"Cada respiração que tomo é uma ordem dela!",
			"Fui reescrito!",
			"A vontade do mestre é a única verdade!",
			"Aceito o esquecimento, se for pela mão dela!",
			"Tu serás ligado ao arquivo infinito dela!",
			"Fica quieto e aceita teu lugar na história dela!"
		};

        public static string GetAttackLine(Mobile defender)
        {
            if (defender == null)
                return Lines[Utility.Random(Lines.Length)];

            string line = Lines[Utility.Random(Lines.Length)];

            if (line.IndexOf("{0}") >= 0)
                return String.Format(line, defender.Name);

            return line;
        }
    }
}
