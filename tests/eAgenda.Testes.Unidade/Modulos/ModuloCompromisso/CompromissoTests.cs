using System.Data;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;

namespace eAgenda.Testes.Unidade.Modulos.ModuloCompromisso
{
    [TestClass]
    public sealed class CompromissoTests
    {
        #region Compromisso

        [TestMethod]
        public void ValidarCompromisso_PresencialDadosValidos()
        {
            // Arrange
            Compromisso compromisso = new Compromisso(
                "Almoço de negócios",
                new DateTime(2026, 8, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Presencial,
                "Restaurante Central",
                null,
                null
            );

            // Act
            List<string> erros = compromisso.Validar();

            // Assert
            Assert.IsEmpty(erros);
        }

        [TestMethod]
        public void ValidarCompromisso_RemotoDadosValidos()
        {
            // Arrange
            Compromisso compromisso = new Compromisso(
                "Almoço de negócios",
                new DateTime(2026, 10, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=1",
                null
            );

            // Act
            List<string> erros = compromisso.Validar();

            // Assert
            Assert.IsEmpty(erros);
        }

        [TestMethod]
        public void ValidarCompromisso_SemContato()
        {
            // Arrange
            Compromisso compromisso = new Compromisso(
                "Almoço de negócios",
                new DateTime(2026, 11, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=5",
                null
            );

            // Act
            List<string> erros = compromisso.Validar();

            // Assert
            Assert.IsEmpty(erros);
        }

        [TestMethod]
        public void ValidarCompromisso_ContatoVinculado()
        {
            // Arrange
            Contato contato = new Contato("João Souza", "joao@123.com", "(51) 995345564", null, null);

            Compromisso compromisso = new Compromisso(
                "Almoço de negócios",
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                contato
            );

            // Act
            List<string> erros = compromisso.Validar();

            // Assert
            Assert.IsEmpty(erros);
        }

        [TestMethod]
        public void ValidarCompromissoRemoto_LinkInvalido()
        {
            // Arrange
            Compromisso compromisso = new Compromisso(
                "Almoço de negócios",
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            // Act
            List<string> erros = compromisso.Validar();

            // Assert
            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Link\" deve conter um endereço de site válido.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_HoraTerminoAnteriorInicio()
        {
            // Arrange
            Compromisso compromisso = new Compromisso(
                "Almoço de negócios",
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(10, 0, 0),
                TipoCompromisso.Presencial,
                "Rua Fernando Ferrari",
                null,
                null
            );

            // Act
            List<string> erros = compromisso.Validar();

            // Assert
            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "A hora de término deve ser posterior à hora de início.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_HoraTerminoIgualInicio()
        {
            // Arrange
            Compromisso compromisso = new Compromisso(
                "Almoço de negócios",
                new DateTime(2026, 12, 8),
                new TimeSpan(9, 0, 0),
                new TimeSpan(9, 0, 0),
                TipoCompromisso.Presencial,
                "Rua Fernando Ferrari",
                null,
                null
            );

            // Act
            List<string> erros = compromisso.Validar();

            // Assert
            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "A hora de término deve ser posterior à hora de início.",
                erros.First()
            );
        }

        #endregion

        #region Campos Obrigatórios em Branco

        [TestMethod]
        public void ValidarCompromisso_AssuntoVazio_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                null,
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Assunto\" deve conter entre 2 e 100 caracteres.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_DataOcorrenciaVazia_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                default,
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Data de Ocorrência\" deve ser preenchido.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_HoraInicioVazia_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                default,
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Hora de Início\" deve ser preenchido.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_HoraTerminoVazia_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 2, 8),
                new TimeSpan(11, 0, 0),
                default,
                TipoCompromisso.Presencial,
                "Rua Silva",
                null,
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Hora de Término\" deve ser preenchido.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_TipoCompromissoVazio_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(13, 0, 0),
                new TimeSpan(14, 0, 0),
                (TipoCompromisso)99,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Tipo de Compromisso\" deve ser preenchido.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_PresencialLocalVazio_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(13, 0, 0),
                new TimeSpan(14, 0, 0),
                TipoCompromisso.Presencial,
                null,
                null,
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Local\" deve ser preenchido para compromissos presenciais.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_RemotoLinkVazio_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(13, 0, 0),
                new TimeSpan(14, 0, 0),
                TipoCompromisso.Remoto,
                null,
                null,
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Link\" deve ser preenchido para compromissos remotos.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_PresencialLocalLongo_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(13, 0, 0),
                new TimeSpan(14, 0, 0),
                TipoCompromisso.Presencial,
                new string('A', 256),
                null,
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Local\" deve conter no máximo 255 caracteres.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarCompromisso_RemotoLinkLongo_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(22, 0, 0),
                new TimeSpan(23, 0, 0),
                TipoCompromisso.Remoto,
                null,
                new string('A', 501),
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Link\" deve conter no máximo 500 caracteres.",
                erros.First()
            );
        }

        #endregion

        #region Assunto

        [TestMethod]
        public void ValidarAssunto_ComCaractereCurto_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                new string('A', 1),
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Assunto\" deve conter entre 2 e 100 caracteres.",
                erros.First()
            );
        }

        [TestMethod]
        public void ValidarAssunto_ComCaractereLimite_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                new string('A', 100),
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(0, erros);
        }

        [TestMethod]
        public void ValidarAssunto_ComCaractereAcimaMaximo_DeveRetornarErro()
        {
            Compromisso compromisso = new Compromisso(
                new string('A', 101),
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            List<string> erros = compromisso.Validar();

            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Assunto\" deve conter entre 2 e 100 caracteres.",
                erros.First()
            );
        }

        #endregion

        #region Atualizar

        [TestMethod]
        public void AtualizarCompromisso_DeveAtualizar_DadosValidos()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "https://meet.google.com/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );


            Compromisso compromissoAtualizado = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(16, 0, 0),
                TipoCompromisso.Presencial,
                "Rua João Souza",
                null,
                null
            );

            compromisso.Atualizar(compromissoAtualizado);

            Assert.AreEqual("Reunião", compromisso.Assunto);
            Assert.AreEqual(new DateTime(2026, 12, 8), compromisso.DataOcorrencia);
            Assert.AreEqual(new TimeSpan(12, 0, 0), compromisso.HoraInicio);
            Assert.AreEqual(new TimeSpan(16, 0, 0), compromisso.HoraTermino);
            Assert.AreEqual(TipoCompromisso.Presencial, compromisso.Tipo);
            Assert.AreEqual("Rua João Souza", compromisso.Local);
            Assert.AreEqual(null, compromisso.Link);
            Assert.AreEqual(null, compromisso.Contato);
        }

        [TestMethod]
        public void AtualizarCompromisso_DeveAtualizar_MesmoHorario()
        {
            Compromisso compromisso = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Presencial,
                "Rua João Souza",
                null,
                null
            );


            Compromisso compromissoAtualizado = new Compromisso(
                "Reunião",
                new DateTime(2026, 12, 8),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                TipoCompromisso.Presencial,
                "Rua João Souza",
                null,
                null
            );

            compromisso.Atualizar(compromissoAtualizado);

            Assert.AreEqual("Reunião", compromisso.Assunto);
            Assert.AreEqual(new DateTime(2026, 12, 8), compromisso.DataOcorrencia);
            Assert.AreEqual(new TimeSpan(12, 0, 0), compromisso.HoraInicio);
            Assert.AreEqual(new TimeSpan(13, 0, 0), compromisso.HoraTermino);
            Assert.AreEqual(TipoCompromisso.Presencial, compromisso.Tipo);
            Assert.AreEqual("Rua João Souza", compromisso.Local);
            Assert.AreEqual(null, compromisso.Link);
            Assert.AreEqual(null, compromisso.Contato);
        }

        #endregion

        [TestMethod]
        public void ValidarCompromisso_RemotoLinkFormatoInvalido_DeveRetornarErro()
        {
            // Arrange
            Compromisso compromisso = new Compromisso(
                "Reunião online",
                new DateTime(2026, 12, 8),
                new TimeSpan(14, 0, 0),
                new TimeSpan(15, 0, 0),
                TipoCompromisso.Remoto,
                null,
                "/vjk-mkpd-zex?pli=1&authuser=7",
                null
            );

            // Act
            List<string> erros = compromisso.Validar();

            // Assert
            Assert.HasCount(1, erros);
            Assert.AreEqual(
                "O campo \"Link\" deve conter um endereço de site válido.",
                erros.First()
            );
        }
    }
}
